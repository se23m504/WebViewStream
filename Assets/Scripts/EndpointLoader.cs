using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.WebView;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EndpointLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject dynamicItem;

    [SerializeField]
    private ServiceDiscovery serviceDiscovery;

    [SerializeField]
    private ServicesListPopulator servicesListPopulator;

    private string apiUrl;
    private bool triedMulticast = false;
    private bool defaultEndpointLoaded = false;
    private List<GameObject> instantiatedItems = new List<GameObject>();
    private HashSet<MdnsService> availableServices = new HashSet<MdnsService>();

    private const string defaultApiUrl = "http://windows.local:5000/api/endpoints";
    private const string defaultEndpoint1 = "http://windows.local:8100/mystream/";
    private const string defaultEndpoint2 = "http://windows.local:8200/mystream/";

    private void Start()
    {
        apiUrl = defaultApiUrl;
        StartCoroutine(LoadEndpoints());
    }

    private Vector3 CalculateNextPosition()
    {
        if (instantiatedItems.Count == 0)
        {
            // return dynamicItem.transform.position;
            return new Vector3(-0.4f, 0.1f, 1);
        }

        GameObject lastItem = instantiatedItems[instantiatedItems.Count - 1];

        Vector3 lastPosition = lastItem.transform.position;
        float itemWidth = GetItemWidth(lastItem);
        return new Vector3(lastPosition.x + itemWidth, lastPosition.y, lastPosition.z);
    }

    private float GetItemWidth(GameObject item)
    {
        Renderer renderer = item.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }

        return 1.0f;
    }

    public void SpawnItem(string url)
    {
        if (dynamicItem != null)
        {
            Vector3 nextPosition = CalculateNextPosition();

            GameObject newItem = Instantiate(
                dynamicItem,
                nextPosition,
                dynamicItem.transform.rotation,
                dynamicItem.transform.parent
            );
            newItem.SetActive(true);

            instantiatedItems.Add(newItem);

            var webView = newItem.GetComponentInChildren<WebView>();

            if (webView != null)
            {
                webView.Load(url);
            }
        }
        else
        {
            Debug.LogError("Dynamic item is not assigned.");
        }
    }

    public List<GameObject> GetInstantiatedItems()
    {
        return instantiatedItems;
    }

    private IEnumerator TryLoadingFromDefaultEndpoints()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(defaultEndpoint1))
        {
            yield return request.SendWebRequest();
            ProcessEndpointResponse(request, defaultEndpoint1, ref defaultEndpointLoaded);
        }

        using (UnityWebRequest request = UnityWebRequest.Get(defaultEndpoint2))
        {
            yield return request.SendWebRequest();
            ProcessEndpointResponse(request, defaultEndpoint2, ref defaultEndpointLoaded);
        }
    }

    private void ProcessEndpointResponse(
        UnityWebRequest request,
        string endpoint,
        ref bool loadedFlag
    )
    {
        if (
            request.result == UnityWebRequest.Result.ConnectionError
            || request.result == UnityWebRequest.Result.ProtocolError
        )
        {
            Debug.LogError($"Error loading from {endpoint}: {request.error}");
        }
        else
        {
            Debug.Log($"Loaded from {endpoint} successfully.");
            SpawnItem(endpoint);
            loadedFlag = true;
        }
    }

    private IEnumerator LoadEndpoints()
    {
        if (!triedMulticast)
        {
            StartListeningForMulticast();
            yield break;
        }

        var request = new UnityWebRequest(apiUrl, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (
            request.result == UnityWebRequest.Result.ConnectionError
            || request.result == UnityWebRequest.Result.ProtocolError
        )
        {
            Debug.LogWarning($"Error loading endpoints: {request.error}");

            if (triedMulticast)
            {
                Debug.LogError("Multicast also failed");
                yield break;
            }

            Debug.LogWarning("Trying to load from default endpoints");
            yield return StartCoroutine(TryLoadingFromDefaultEndpoints());
        }

        if (defaultEndpointLoaded)
        {
            Debug.Log("At least one default endpoint loaded successfully");
            yield break;
        }

        var json = request.downloadHandler.text;
        json = "{\"Items\":" + json + "}";
        Debug.Log($"Received JSON: {json}");

        Endpoint[] endpoints = JsonHelper.FromJson<Endpoint>(json);

        if (endpoints.Length == 0)
        {
            Debug.LogError("Parsed endpoints are empty");
        }
        else
        {
            foreach (var endpoint in endpoints)
            {
                if (endpoint.url == null || endpoint.url.Length == 0)
                {
                    Debug.LogWarning($"Endpoint URL is null for endpoint");
                    continue;
                }
                SpawnItem(endpoint.url);
            }
        }
    }

    private void StartListeningForMulticast()
    {
        Debug.Log("Starting multicast discovery for endpoints");

        triedMulticast = true;
        serviceDiscovery.StartListening(
            (service) =>
            {
                bool wasAdded = availableServices.Add(service);
                if (wasAdded)
                {
                    AddServiceToTable(service);
                }
            }
        );
    }

    private void AddServiceToTable(MdnsService service)
    {
        servicesListPopulator.AddItemFromService(
            service,
            () =>
            {
                apiUrl = $"http://{service.Host}:{service.Port}{service.Path}";
                StartCoroutine(LoadEndpoints());
            }
        );
    }

    public void ClearServices()
    {
        availableServices.Clear();
        servicesListPopulator.RemoveAllItems();
    }

    public void ReloadEndpoints()
    {
        triedMulticast = false;
        StartCoroutine(LoadEndpoints());
    }

    [Serializable]
    public class Endpoint
    {
        public int id;
        public string url;
    }

    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}

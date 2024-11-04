using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.WebView;

public class EndpointLoader : MonoBehaviour
{
    [SerializeField]
    private WebView webView1;

    [SerializeField]
    private WebView webView2;

    [SerializeField]
    private ServiceDiscovery serviceDiscovery;

    [SerializeField]
    private ServicesListPopulator servicesListPopulator;

    private string apiUrl;
    private bool triedMulticast = false;
    private bool defaultEndpointLoaded = false;
    private HashSet<MdnsService> availableServices = new HashSet<MdnsService>();

    private const string defaultApiUrl = "http://windows.local:5000/api/endpoints";
    private const string defaultEndpoint1 = "http://windows.local:8100/mystream/";
    private const string defaultEndpoint2 = "http://windows.local:8200/mystream/";

    private void Start()
    {
        apiUrl = defaultApiUrl;
        StartCoroutine(LoadEndpoints());
    }

    private IEnumerator TryLoadingFromDefaultEndpoints()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(defaultEndpoint1))
        {
            yield return request.SendWebRequest();
            ProcessEndpointResponse(request, webView1, defaultEndpoint1, ref defaultEndpointLoaded);
        }

        using (UnityWebRequest request = UnityWebRequest.Get(defaultEndpoint2))
        {
            yield return request.SendWebRequest();
            ProcessEndpointResponse(request, webView2, defaultEndpoint2, ref defaultEndpointLoaded);
        }
    }

    private void ProcessEndpointResponse(UnityWebRequest request, WebView webView, string endpoint, ref bool loadedFlag)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error loading from {endpoint}: {request.error}");
        }
        else
        {
            Debug.Log($"Loaded from {endpoint} successfully.");
            webView.Load(endpoint);
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

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
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
            webView1.Load(endpoints[0].url ?? defaultEndpoint1);
            webView2.Load(endpoints[1].url ?? defaultEndpoint2);
        }
    }

    private void StartListeningForMulticast()
    {
        Debug.Log("Starting multicast discovery for endpoints");

        triedMulticast = true;
        serviceDiscovery.StartListening((service) =>
        {
            bool wasAdded = availableServices.Add(service);
            if (wasAdded)
            {
                AddServiceToTable(service);
            }
        });
    }

    private void AddServiceToTable(MdnsService service)
    {
        servicesListPopulator.AddItemFromService(service, () =>
        {
            apiUrl = $"http://{service.Host}:{service.Port}{service.Path}";
            StartCoroutine(LoadEndpoints());
        });
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

    private void UseDefaultEndpoints()
    {
        webView1.Load(defaultEndpoint1);
        webView2.Load(defaultEndpoint2);
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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.WebView;

public class EndpointLoader : MonoBehaviour
{
    public WebView webView1;
    public WebView webView2;
    public ServiceDiscovery serviceDiscovery;

    private bool triedMulticast = false;
    private string apiUrl = "http://windows.loca:5000/api/endpoints";
    private const string defaultEndpoint1 = "http://windows.local:8100/mystream/";
    private const string defaultEndpoint2 = "http://windows.local:8200/mystream/";

    private void Start()
    {
        StartCoroutine(LoadEndpoints());
    }

    private IEnumerator LoadEndpoints()
    {
        var request = new UnityWebRequest(apiUrl, UnityWebRequest.kHttpVerbGET);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogWarning($"Error loading endpoints: {request.error}");

            StartListeningForMulticast();
            yield break;
        }

        var json = request.downloadHandler.text;
        json = "{\"Items\":" + json + "}";
        Debug.Log($"Received JSON: {json}");

        Endpoint[] endpoints = JsonHelper.FromJson<Endpoint>(json);

        if (endpoints.Length == 0)
        {
            Debug.LogError("Parsed endpoints are empty.");
            StartListeningForMulticast();
        }
        else
        {
            webView1.Load(endpoints[0].url ?? defaultEndpoint1);
            webView2.Load(endpoints[1].url ?? defaultEndpoint2);
        }
    }

    private void StartListeningForMulticast()
    {
        if (triedMulticast)
        {
            Debug.LogWarning("Multicast also failed. Using default endpoints.");
            UseDefaultEndpoints();
            return;
        }

        Debug.Log("Starting multicast discovery for endpoints");

        triedMulticast = true;
        serviceDiscovery.StartListening((ipAddress, port) =>
        {
            apiUrl = $"http://{ipAddress}:{port}/api/endpoints";
            StartCoroutine(LoadEndpoints());
        });
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

using Microsoft.MixedReality.WebView;
using System;
using TMPro;
using UnityEngine;

public class WebViewBrowser : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField URLField;

    private void Start()
    {
        var webViewComponent = gameObject.GetComponent<WebView>();
        webViewComponent.GetWebViewWhenReady(
            (IWebView webView) =>
            {
                URLField.onSubmit.AddListener((text) => LoadUrl(webView));

                webView.Navigated += OnNavigated;

                if (webView.Page != null)
                {
                    URLField.text = webView.Page.AbsoluteUri;
                }
            }
        );
    }

    private void OnNavigated(string path)
    {
        URLField.text = path;
    }

    private void LoadUrl(IWebView webView)
    {
        if (Uri.TryCreate(URLField.text, UriKind.Absolute, out Uri uriResult))
        {
            webView.Load(uriResult);
        }
        else
        {
            Debug.LogWarning("Invalid URL entered.");
        }
    }
}

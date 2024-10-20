using Microsoft.MixedReality.WebView;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class WebViewBrowser : MonoBehaviour
{
    public Button GoButton;
    public TMP_InputField URLField;

    private void Start()
    {
        var webViewComponent = gameObject.GetComponent<WebView>();
        webViewComponent.GetWebViewWhenReady((IWebView webView) =>
        {
            GoButton.onClick.AddListener(() => webView.Load(new Uri(URLField.text)));

            webView.Navigated += OnNavigated;

            if (webView.Page != null)
            {
                URLField.text = webView.Page.AbsoluteUri;
            }
        });
    }

    private void OnNavigated(string path)
    {
        URLField.text = path;
    }
}
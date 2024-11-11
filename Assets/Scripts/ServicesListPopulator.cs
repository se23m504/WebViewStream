using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;
using UnityEngine;

public class ServicesListPopulator : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollView;

    [SerializeField]
    private GameObject dynamicItem;

    [SerializeField]
    private GridObjectCollection gridObjectCollection;

    private bool isVisible = true;

    private const string apiUrlPrefix = "API URL: ";
    private const string hostPrefix = "Host: ";

    public void AddItemFromService(MdnsService service, Action action)
    {
        GameObject itemInstance = Instantiate(dynamicItem, gridObjectCollection.transform);
        itemInstance.SetActive(true);

        Debug.Log($"Adding service to table: {service}");
        TextMeshPro[] textMeshes = itemInstance.GetComponentsInChildren<TextMeshPro>();
        if (textMeshes.Length < 2)
        {
            Debug.LogError("Not enough text meshes found in dynamic item");
            return;
        }
        textMeshes[0].text = $"{apiUrlPrefix}http://{service.IpAddress}:{service.Port}{service.Path}";
        textMeshes[1].text = $"{hostPrefix}{service.Host}";
        itemInstance
            .GetComponentInChildren<Interactable>()
            .OnClick.AddListener(() =>
            {
                Debug.Log($"Clicked on service: {service.Host}");
                action.Invoke();
                ToggleVisibility();
            });

        gridObjectCollection.UpdateCollection();
        scrollView.UpdateContent();
    }

    public void RemoveAllItems()
    {
        foreach (Transform child in gridObjectCollection.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Removed all services from table");
        gridObjectCollection.UpdateCollection();
        scrollView.UpdateContent();
    }

    public void RemoveItemByService(MdnsService service)
    {
        string apiUrl = $"{apiUrlPrefix}http://{service.IpAddress}:{service.Port}{service.Path}";
        string hostname = $"{hostPrefix}{service.Host}";

        foreach (Transform child in gridObjectCollection.transform)
        {
            TextMeshPro[] textMeshes = child.GetComponentsInChildren<TextMeshPro>();
            if (textMeshes.Length >= 2 && textMeshes[0].text == apiUrl && textMeshes[1].text == hostname)
            {
                Debug.Log($"Removing service from table: {service}");
                Destroy(child.gameObject);
                break;
            }
        }

        gridObjectCollection.UpdateCollection();
        scrollView.UpdateContent();
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;

        gameObject.SetActive(isVisible);
    }
}

using TMPro;
using System;
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ServicesListPopulator : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollView;

    [SerializeField]
    private GameObject dynamicItem;

    [SerializeField]
    private GridObjectCollection gridObjectCollection;

    private bool isVisible = true;

    public void AddItemFromService(MdnsService service, Action action)
    {
        GameObject itemInstance = Instantiate(dynamicItem, gridObjectCollection.transform);
        itemInstance.SetActive(true);

        Debug.Log($"Adding service to table: {service}");
        TextMeshPro[] textMeshes = itemInstance.GetComponentsInChildren<TextMeshPro>();
        textMeshes[0].text = service.Host + ":" + service.Port + service.Path;
        textMeshes[1].text = service.IpAddress;
        itemInstance.GetComponentInChildren<Interactable>().OnClick.AddListener(() =>
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
        string fullAddress = service.Host + ":" + service.Port + service.Path;
        string ipAddress = service.IpAddress;

        foreach (Transform child in gridObjectCollection.transform)
        {
            TextMeshPro[] textMeshes = child.GetComponentsInChildren<TextMeshPro>();
            if (textMeshes.Length >= 2 && textMeshes[0].text == fullAddress && textMeshes[1].text == ipAddress)
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

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = isVisible;
        }
    }
}
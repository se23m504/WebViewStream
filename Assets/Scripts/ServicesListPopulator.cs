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
        });

        gridObjectCollection.UpdateCollection();
    }
}
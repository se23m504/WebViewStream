using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfigureNavBar : MonoBehaviour
{
    [SerializeField]
    private EndpointLoader endpointLoader;

    private bool isVisible = false;

    public void ToggleVisibilityMethod()
    {
        List<GameObject> canvases = endpointLoader.GetInstantiatedItems();
        isVisible = !isVisible;
        foreach (GameObject canvas in canvases)
        {
            TMP_InputField inputField = canvas.GetComponentInChildren<TMP_InputField>(true);
            if (inputField != null)
            {
                Debug.Log("Setting address field visibility to " + isVisible);
                inputField.gameObject.SetActive(isVisible);
            }

            BoxCollider boxCollider = canvas.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.size = new Vector3(boxCollider.size.x, isVisible ? 400 : 370, boxCollider.size.z);
                boxCollider.center = new Vector3(0, isVisible ? 0 : -16, 0);
            }
        }
    }
}

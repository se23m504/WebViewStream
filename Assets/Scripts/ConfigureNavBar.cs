using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigureNavBar : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas1;

    [SerializeField]
    private GameObject canvas2;

    [SerializeField]
    private GameObject addressField1;

    [SerializeField]
    private GameObject addressField2;

    private BoxCollider boxCollider1;
    private BoxCollider boxCollider2;

    private bool isVisible = false;

    private void Start()
    {
        boxCollider1 = canvas1.GetComponent<BoxCollider>();
        boxCollider2 = canvas2.GetComponent<BoxCollider>();
    }

    public void ToggleVisibilityMethod()
    {
        isVisible = !isVisible;
        addressField1.SetActive(isVisible);
        addressField2.SetActive(isVisible);

        if (boxCollider1 != null)
        {
            boxCollider1.size = new Vector3(boxCollider1.size.x, isVisible ? 400 : 370, boxCollider1.size.z);
            boxCollider1.center = new Vector3(0, isVisible ? 0: -16, 0);
        }

        if (boxCollider2 != null)
        {
            boxCollider2.size = new Vector3(boxCollider2.size.x, isVisible ? 400 : 370, boxCollider2.size.z);
            boxCollider2.center = new Vector3(0, isVisible ? 0: -16, 0);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfigureNavBar : MonoBehaviour
{
    public GameObject canvas1;
    public GameObject canvas2;

    public GameObject addressField1;
    public GameObject addressField2;

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
            boxCollider1.size = new Vector3(boxCollider1.size.x, isVisible ? 400 : 300, boxCollider1.size.z);
            boxCollider1.center = new Vector3(isVisible ? 0: -16, 0, 0);
        }

        if (boxCollider2 != null)
        {
            boxCollider2.size = new Vector3(boxCollider1.size.x, isVisible ? 400 : 300, boxCollider2.size.z);
            boxCollider2.center = new Vector3(isVisible ? 0: -16, 0, 0);
        }
    }
}


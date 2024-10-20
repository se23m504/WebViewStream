using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureNavBar : MonoBehaviour
{
    public GameObject addressField1;
    public GameObject goButton1;

    public GameObject addressField2;
    public GameObject goButton2;

    private bool isVisible = false;

    public void ToggleVisibilityMethod()
    {
        isVisible = !isVisible;
        addressField1.SetActive(isVisible);
        goButton1.SetActive(isVisible);
        addressField2.SetActive(isVisible);
        goButton2.SetActive(isVisible);
    }
}


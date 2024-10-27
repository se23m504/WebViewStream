using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class ConfigurePointer : MonoBehaviour
{
    private bool handRayPointerEnabled = true;

    public void ToggleHandRayPointer()
    {
        if (handRayPointerEnabled)
        {
            DisableHandRayPointer();
            handRayPointerEnabled = false;
        }
        else
        {
            EnableHandRayPointer();
            handRayPointerEnabled = true;
        }
    }

    public void EnableHandRayPointer()
    {
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOn);
    }

    public void DisableHandRayPointer()
    {
        PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
    }
}


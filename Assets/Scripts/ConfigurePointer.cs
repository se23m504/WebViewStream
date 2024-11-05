using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

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

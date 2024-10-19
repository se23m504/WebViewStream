using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

public class ConfigurePointer : MonoBehaviour
{
    void Start()
    {
        // PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff);
        // PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOff);
        // PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff); --> This is the one we want to disable/enable
    }

    // make a toggle method for the hand ray pointer. use a bool to keep track of the state
    // if the bool is true, disable the hand ray pointer
    // if the bool is false, enable the hand ray pointer
    // by default it is true.
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


using Microsoft.MixedReality.Toolkit.Input;
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

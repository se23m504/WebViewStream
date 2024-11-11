using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace WebViewStream
{
    public class ConfigurePointer : MonoBehaviour
    {
        private bool handRayPointerEnabled = true;

        /// <summary>
        /// Toggles the hand ray pointer on and off.
        /// </summary>
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

        private void EnableHandRayPointer()
        {
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOn);
        }

        private void DisableHandRayPointer()
        {
            PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
        }
    }
}
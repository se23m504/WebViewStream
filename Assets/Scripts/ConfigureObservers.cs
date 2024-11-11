using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

public class ConfigureObservers : MonoBehaviour
{
    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;

    private void Start()
    {
        spatialAwarenessSystem =
            MixedRealityToolkit.Instance.GetService<IMixedRealitySpatialAwarenessSystem>();

        if (spatialAwarenessSystem != null)
        {
            spatialAwarenessSystem.SuspendObservers();
            Debug.Log("Spatial observers suspended");
        }
        else
        {
            Debug.LogWarning("SAS is not available");
        }
    }
}

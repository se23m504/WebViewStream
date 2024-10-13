using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;

public class ConfigureObservers : MonoBehaviour
{
    private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem;

    void Start()
    {
        spatialAwarenessSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySpatialAwarenessSystem>();

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


using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class ConfigureOrbital : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas1;

    [SerializeField]
    private GameObject canvas2;

    // public List<GameObject> canvases;

    private Orbital orbital1;
    private Orbital orbital2;

    private SolverHandler solverHandler1;
    private SolverHandler solverHandler2;

    void Start()
    {
        if (canvas1 != null)
        {
            orbital1 = canvas1.GetComponent<Orbital>();
            solverHandler1 = canvas1.GetComponent<SolverHandler>();
        }

        if (canvas2 != null)
        {
            orbital2 = canvas2.GetComponent<Orbital>();
            solverHandler2 = canvas2.GetComponent<SolverHandler>();
        }

        if (
            orbital1 == null
            || solverHandler1 == null
            || orbital2 == null
            || solverHandler2 == null
        )
        {
            Debug.LogError(
                "One or both Canvas objects are missing an Orbital or SolverHandler component."
            );
        }
    }

    public void ToggleOrbitalScript()
    {
        ToggleOrbitalForCanvas(orbital1, solverHandler1);
        ToggleOrbitalForCanvas(orbital2, solverHandler2);
    }

    private void ToggleOrbitalForCanvas(Orbital orbital, SolverHandler solverHandler)
    {
        if (orbital != null && solverHandler != null)
        {
            orbital.enabled = !orbital.enabled;

            if (orbital.enabled)
            {
                Vector3 headPosition = Camera.main.transform.position;
                Quaternion headRotation = Camera.main.transform.rotation;
                Vector3 relativePosition =
                    Quaternion.Inverse(headRotation) * (orbital.transform.position - headPosition);

                orbital.LocalOffset = relativePosition;

                solverHandler.UpdateSolvers = true;
            }
            else
            {
                solverHandler.UpdateSolvers = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class ConfigureOrbital : MonoBehaviour
{
    [SerializeField]
    private EndpointLoader endpointLoader;

    private bool orbitalEnabled = false;

    public void ToggleOrbital()
    {
        orbitalEnabled = !orbitalEnabled;
        List<GameObject> canvases = endpointLoader.GetInstantiatedItems();

        foreach (GameObject canvas in canvases)
        {
            Orbital orbital = canvas.GetComponent<Orbital>();
            SolverHandler solverHandler = canvas.GetComponent<SolverHandler>();

            if (orbital != null && solverHandler != null)
            {
                orbital.enabled = orbitalEnabled;

                if (orbitalEnabled)
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

    public void RotateCanvasToFaceUser()
    {
        List<GameObject> canvases = endpointLoader.GetInstantiatedItems();

        foreach (GameObject canvas in canvases)
        {
            Vector3 directionToCamera = canvas.transform.position - Camera.main.transform.position;
            canvas.transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}

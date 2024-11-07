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

    public void CenterCanvasesToUser()
    {
        List<GameObject> canvases = endpointLoader.GetInstantiatedItems();

        Vector3 localOffset = new Vector3(-0.4f, 0.1f, 1f);

        foreach (GameObject canvas in canvases)
        {
            Transform cameraTransform = Camera.main.transform;

            canvas.transform.position = cameraTransform.position + cameraTransform.TransformDirection(localOffset);
            // canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - cameraTransform.position);
            // canvas.transform.rotation = Quaternion.LookRotation(-cameraTransform.position);
            // canvas.transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            canvas.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);

            localOffset = new Vector3(localOffset.x + GetItemWidth(canvas), localOffset.y, localOffset.z);
        }
    }

    private float GetItemWidth(GameObject item)
    {
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            return rectTransform.rect.width * rectTransform.lossyScale.x + 0.2f;
        }

        return 0.8f;
    }
}

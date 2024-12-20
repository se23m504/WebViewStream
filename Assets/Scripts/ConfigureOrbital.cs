using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace WebViewStream
{
    public class ConfigureOrbital : MonoBehaviour
    {
        [SerializeField]
        private EndpointLoader endpointLoader;

        private bool orbitalEnabled = false;

        /// <summary>
        /// Toggles the orbital behavior (solver) of the canvases.
        /// </summary>
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

        /// <summary>
        /// Rotates the canvases to face the user.
        /// </summary>
        public void RotateCanvasToFaceUser()
        {
            List<GameObject> canvases = endpointLoader.GetInstantiatedItems();

            foreach (GameObject canvas in canvases)
            {
                Vector3 directionToCamera = canvas.transform.position - Camera.main.transform.position;
                canvas.transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }

        /// <summary>
        /// Centers the canvases in front of the user.
        /// </summary>
        public void CenterCanvasesToUser()
        {
            List<GameObject> canvases = endpointLoader.GetInstantiatedItems();

            Vector3 localOffset = new Vector3(-0.3f, 0.1f, 1.5f);

            foreach (GameObject canvas in canvases)
            {
                Transform cameraTransform = Camera.main.transform;

                canvas.transform.position =
                    cameraTransform.position + cameraTransform.TransformDirection(localOffset);
                canvas.transform.rotation = Quaternion.LookRotation(
                    cameraTransform.forward,
                    cameraTransform.up
                );

                localOffset = new Vector3(
                    localOffset.x + endpointLoader.GetItemWidth(canvas),
                    localOffset.y,
                    localOffset.z
                );
            }
        }
    }
}

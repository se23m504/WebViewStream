using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace WebViewStream
{
    public class DialogHandler : MonoBehaviour
    {
        [SerializeField]
        private GameObject dialogPrefab;

        /// <summary>
        /// Opens a dialog with a title, question, and action.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="question"></param>
        /// <param name="action"></param>
        public void OpenDialog(string title, string question, Action action)
        {
            Dialog dialog = Dialog.Open(
                dialogPrefab,
                DialogButtonType.Yes | DialogButtonType.No,
                title,
                question,
                true
            );
            if (dialog != null)
            {
                Follow follow = dialog.GetComponent<Follow>();
                follow.MinDistance = 0.5f;
                follow.DefaultDistance = 0.5f;
                follow.MaxDistance = 0.5f;

                dialog.OnClosed += (x) =>
                {
                    if (x.Result == DialogButtonType.Yes)
                    {
                        action?.Invoke();
                    }
                };
            }
        }
    }
}

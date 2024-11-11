using System;
using Microsoft.MixedReality.Toolkit.UI;
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
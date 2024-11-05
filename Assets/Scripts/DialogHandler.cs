using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class DialogHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogPrefab;

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
            // myDialog.OnClosed += OnClosedDialogEvent;
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

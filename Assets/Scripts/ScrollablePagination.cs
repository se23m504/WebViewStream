using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ScrollablePagination : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollView;

    public void ScrollByTier(int amount)
    {
        if (scrollView == null)
        {
            Debug.LogError("ScrollingObjectCollection is not set.");
            return;
        }

        scrollView.MoveByTiers(amount);
    }
}
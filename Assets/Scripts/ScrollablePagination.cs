using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class ScrollablePagination : MonoBehaviour
{
    [SerializeField]
    private ScrollingObjectCollection scrollView;

    public ScrollingObjectCollection ScrollView
    {
        get
        {
            if (scrollView == null)
            {
                scrollView = GetComponent<ScrollingObjectCollection>();
            }
            return scrollView;
        }
        set
        {
            scrollView = value;
        }
    }

    public void ScrollByTier(int amount)
    {
        Debug.Assert(ScrollView != null, "Scroll view needs to be defined before using pagination.");
        scrollView.MoveByTiers(amount);
    }
}
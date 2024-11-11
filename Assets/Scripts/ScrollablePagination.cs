using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace WebViewStream
{
    public class ScrollablePagination : MonoBehaviour
    {
        [SerializeField]
        private ScrollingObjectCollection scrollView;

        /// <summary>
        /// Scrolls the collection by a specified amount.
        /// </summary>
        /// <param name="amount"></param>
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
}

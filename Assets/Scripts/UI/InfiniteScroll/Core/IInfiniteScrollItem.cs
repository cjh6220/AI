using UnityEngine;

namespace InfiniteScroll.Core
{
    /// <summary>
    /// Interface for items used in the Infinite Scroll system.
    /// Handles lifecycle and data binding.
    /// </summary>
    public interface IInfiniteScrollItem
    {
        /// <summary>
        /// Unique identifier for the item (e.g., the index in the data source).
        /// </summary>
        int DataIndex { get; set; }

        /// <summary>
        /// Called when the item is being prepared for display with new data.
        /// </summary>
        void OnBind(object data);

        /// <summary>
        /// Called when the item is recycled back to the pool.
        /// </summary>
        void OnUnbind();

        /// <summary>
        /// The RectTransform of the item.
        /// </summary>
        RectTransform RectTransform { get; }
    }
}

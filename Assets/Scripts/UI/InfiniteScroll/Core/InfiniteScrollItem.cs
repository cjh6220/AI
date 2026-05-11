using UnityEngine;

namespace InfiniteScroll.Core
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class InfiniteScrollItem : MonoBehaviour, IInfiniteScrollItem
    {
        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public int DataIndex { get; set; } = -1;
        public System.Action<int> OnClickAction;

        public abstract void OnBind(object data);

        public virtual void OnUnbind()
        {
            DataIndex = -1;
            OnClickAction = null;
        }

        protected void OnItemClicked()
        {
            OnClickAction?.Invoke(DataIndex);
        }

        /// <summary>
        /// Utility method to set the anchored position of the item.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfiniteScroll.Core
{
    [RequireComponent(typeof(ScrollRect))]
    public class InfiniteScrollController : MonoBehaviour
    {
        public enum Direction { Vertical, Horizontal }

        [Header("Settings")]
        [SerializeField] private Direction scrollDirection = Direction.Vertical;
        [SerializeField] private InfiniteScrollItem itemPrefab;
        [SerializeField] private float itemSize = 100f;
        [SerializeField] private float spacing = 0f;
        [SerializeField] private RectOffset padding;
        [SerializeField] private int bufferItems = 2;
        [SerializeField] private int initialPoolSize = 15;

        private ScrollRect _scrollRect;
        private RectTransform _viewport;
        private RectTransform _content;
        private InfiniteScrollPool _pool;
        
        // Data and active views
        private readonly Dictionary<int, InfiniteScrollItem> _activeItems = new Dictionary<int, InfiniteScrollItem>();
        private int _totalCount = 0;
        private Func<int, object> _dataGetter;

        public event Action<int> OnItemClicked;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _viewport = _scrollRect.viewport;
            _content = _scrollRect.content;

            if (itemPrefab == null)
            {
                Debug.LogError("[InfiniteScroll] Item Prefab is not assigned!");
                enabled = false;
                return;
            }

            SetupContentTransform();
            _pool = new InfiniteScrollPool(itemPrefab, _content, initialPoolSize);
            _scrollRect.onValueChanged.AddListener(OnScroll);
        }

        private void SetupContentTransform()
        {
            // Force content anchors and pivot to facilitate manual positioning
            if (scrollDirection == Direction.Vertical)
            {
                _content.anchorMin = new Vector2(0, 1);
                _content.anchorMax = new Vector2(1, 1);
                _content.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                _content.anchorMin = new Vector2(0, 0);
                _content.anchorMax = new Vector2(0, 1);
                _content.pivot = new Vector2(0, 0.5f);
            }
            _content.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Initializes the scroll with data.
        /// </summary>
        /// <param name="totalCount">Total number of items in the dataset.</param>
        /// <param name="dataGetter">Delegate to retrieve data for a specific index.</param>
        public void Initialize(int totalCount, Func<int, object> dataGetter)
        {
            _totalCount = totalCount;
            _dataGetter = dataGetter;

            ClearActiveItems();
            UpdateContentSize();
            
            // Force first update
            Refresh();
        }

        public void Refresh()
        {
            OnScroll(_scrollRect.normalizedPosition);
        }

        private void OnScroll(Vector2 normalizedPos)
        {
            if (_totalCount <= 0 || _dataGetter == null) return;

            // Calculate visible range
            var range = GetVisibleRange();
            
            // Recycle items that are out of range
            RecycleOutOfRange(range.start, range.end);

            // Bind new items that came into range
            UpdateVisibleItems(range.start, range.end);
        }

        private (int start, int end) GetVisibleRange()
        {
            float contentPos = (scrollDirection == Direction.Vertical) 
                ? _content.anchoredPosition.y - padding.top 
                : -_content.anchoredPosition.x - padding.left;

            float viewportSize = (scrollDirection == Direction.Vertical) 
                ? _viewport.rect.height 
                : _viewport.rect.width;

            float step = itemSize + spacing;
            
            int start = Mathf.FloorToInt(contentPos / step) - bufferItems;
            int end = Mathf.CeilToInt((contentPos + viewportSize) / step) + bufferItems;

            start = Mathf.Clamp(start, 0, _totalCount - 1);
            end = Mathf.Clamp(end, 0, _totalCount - 1);

            return (start, end);
        }

        private readonly List<int> _recycleIndicesBuffer = new List<int>();

        private void RecycleOutOfRange(int start, int end)
        {
            _recycleIndicesBuffer.Clear();
            foreach (var pair in _activeItems)
            {
                if (pair.Key < start || pair.Key > end)
                {
                    _recycleIndicesBuffer.Add(pair.Key);
                }
            }

            for (int i = 0; i < _recycleIndicesBuffer.Count; i++)
            {
                int index = _recycleIndicesBuffer[i];
                var item = _activeItems[index];
                _activeItems.Remove(index);
                _pool.Return(item);
            }
        }

        private void UpdateVisibleItems(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if (!_activeItems.ContainsKey(i))
                {
                    var item = _pool.Get();
                    item.DataIndex = i;
                    item.OnClickAction = HandleItemClick;
                    item.OnBind(_dataGetter(i));
                    item.SetPosition(CalculateItemPosition(i));
                    _activeItems.Add(i, item);
                }
                }
                }

                private void HandleItemClick(int index)
                {
                OnItemClicked?.Invoke(index);
                }

        private Vector2 CalculateItemPosition(int index)
        {
            float step = itemSize + spacing;
            if (scrollDirection == Direction.Vertical)
            {
                return new Vector2(0, -padding.top - (index * step));
            }
            else
            {
                return new Vector2(padding.left + (index * step), 0);
            }
        }

        private void UpdateContentSize()
        {
            float totalSize = (itemSize * _totalCount) + (spacing * Mathf.Max(0, _totalCount - 1));
            
            if (scrollDirection == Direction.Vertical)
            {
                totalSize += padding.top + padding.bottom;
                _content.sizeDelta = new Vector2(_content.sizeDelta.x, totalSize);
            }
            else
            {
                totalSize += padding.left + padding.right;
                _content.sizeDelta = new Vector2(totalSize, _content.sizeDelta.y);
            }
        }

        private void ClearActiveItems()
        {
            foreach (var item in _activeItems.Values)
            {
                _pool.Return(item);
            }
            _activeItems.Clear();
        }
        
        private void OnDestroy()
        {
            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.RemoveListener(OnScroll);
            }
        }
    }
}

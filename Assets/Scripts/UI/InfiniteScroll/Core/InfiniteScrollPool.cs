using System.Collections.Generic;
using UnityEngine;

namespace InfiniteScroll.Core
{
    public class InfiniteScrollPool
    {
        private readonly InfiniteScrollItem _prefab;
        private readonly RectTransform _parent;
        private readonly Stack<InfiniteScrollItem> _pool = new Stack<InfiniteScrollItem>();

        public InfiniteScrollPool(InfiniteScrollItem prefab, RectTransform parent, int initialCapacity = 10)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialCapacity; i++)
            {
                CreateNewItem();
            }
        }

        private InfiniteScrollItem CreateNewItem()
        {
            var item = Object.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(false);
            _pool.Push(item);
            return item;
        }

        public InfiniteScrollItem Get()
        {
            var item = _pool.Count > 0 ? _pool.Pop() : Object.Instantiate(_prefab, _parent);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(InfiniteScrollItem item)
        {
            item.OnUnbind();
            item.gameObject.SetActive(false);
            _pool.Push(item);
        }
    }
}

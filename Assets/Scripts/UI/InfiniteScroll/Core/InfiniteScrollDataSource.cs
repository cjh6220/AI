using System.Collections.Generic;

namespace InfiniteScroll.Core
{
    /// <summary>
    /// Base class for the data source.
    /// Can be extended to support complex data fetching (e.g., from a database or API).
    /// </summary>
    public class InfiniteScrollDataSource<T>
    {
        private readonly List<T> _data = new List<T>();

        public int Count => _data.Count;

        public void SetData(IEnumerable<T> newData)
        {
            _data.Clear();
            if (newData != null)
            {
                _data.AddRange(newData);
            }
        }

        public T GetData(int index)
        {
            if (index < 0 || index >= _data.Count) return default;
            return _data[index];
        }

        public void AddData(T item) => _data.Add(item);
        
        public void Clear() => _data.Clear();
    }
}

using System.Collections.Generic;
using UnityEngine;
using InfiniteScroll.Core;

namespace InfiniteScroll.Examples
{
    public class InfiniteScrollTest : MonoBehaviour
    {
        [SerializeField] private InfiniteScrollController scrollController;
        [SerializeField] private int itemCount = 1000;

        private InfiniteScrollDataSource<MyItemData> _dataSource;

        private void Start()
        {
            _dataSource = new InfiniteScrollDataSource<MyItemData>();

            // Generate mock data
            var dataList = new List<MyItemData>();
            for (int i = 0; i < itemCount; i++)
            {
                dataList.Add(new MyItemData
                {
                    Id = i,
                    Title = $"Item #{i}",
                    Description = $"This is the description for item index {i}."
                });
            }

            _dataSource.SetData(dataList);

            // Initialize controller
            scrollController.Initialize(_dataSource.Count, index => _dataSource.GetData(index));
            
            // Subscribe to events
            scrollController.OnItemClicked += OnItemClicked;
        }

        private void OnItemClicked(int index)
        {
            var data = _dataSource.GetData(index);
            Debug.Log($"[InfiniteScrollTest] Clicked on: {data.Title} (Index: {index})");
        }
    }
}

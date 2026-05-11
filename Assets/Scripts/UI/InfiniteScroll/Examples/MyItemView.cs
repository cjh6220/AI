using UnityEngine;
using UnityEngine.UI;
using InfiniteScroll.Core;
using TMPro;

namespace InfiniteScroll.Examples
{
    public class MyItemView : InfiniteScrollItem
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Button actionButton;

        private void Awake()
        {
            if (actionButton != null)
            {
                actionButton.onClick.AddListener(OnItemClicked);
            }
        }

        public override void OnBind(object data)
        {
            if (data is MyItemData itemData)
            {
                titleText.text = itemData.Title;
                descriptionText.text = itemData.Description;
            }
        }
    }
}

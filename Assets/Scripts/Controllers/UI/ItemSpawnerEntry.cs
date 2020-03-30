using Controllers.Dev;
using Models.Items;
using TMPro;
using UnityEngine;

namespace Controllers.UI
{
    public class ItemSpawnerEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemNameText;

        [SerializeField] private TMP_Text x1ButtonText;
        [SerializeField] private TMP_Text maxButtonText;

        [SerializeField] private Color buttonSelectedColor;
        [SerializeField] private Color buttonDeselectedColor;

        public Item Item { get; private set; }

        public int Quantity { get; private set; } = 1;

        private ItemsDevTool tool;

        public void Set(Item _item, ItemsDevTool _toolInstance)
        {
            Item = _item;
            itemNameText.text = Item.ItemName;
            tool = _toolInstance;
        }

        public void OnItemClick()
        {
            tool.SelectedItemSpawner = this;
        }

        public void OnX1Click()
        {
            x1ButtonText.color = buttonSelectedColor;
            maxButtonText.color = buttonDeselectedColor;

            Quantity = 1;
        }

        public void OnMaxClick()
        {
            x1ButtonText.color = buttonDeselectedColor;
            maxButtonText.color = buttonSelectedColor;

            Quantity = Item.MaxStackSize;
        }
    }
}
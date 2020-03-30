using Controllers.UI;
using Models.Items;
using Models.Map;
using Models.Map.Tiles;
using UnityEngine;

namespace Controllers.Dev
{
    public class ItemsDevTool : MonoBehaviour
    {
        public ItemSpawnerEntry SelectedItemSpawner { get; set; }

        [SerializeField] private GameObject panelRoot;

        [SerializeField] private GameObject itemSpawnerEntryPrefab;

        [SerializeField] private Transform entryContainer;

        private void Start()
        {
            MouseController.Instance.mouseClickEvent += MouseClick;

            foreach (var item in ItemManager.Items)
            {
                var entry = Instantiate(itemSpawnerEntryPrefab, entryContainer);
                entry.GetComponent<ItemSpawnerEntry>().Set(item, this);
            }
        }

        public void Toggle()
        {
            panelRoot.SetActive(!panelRoot.activeSelf);
        }

        private void MouseClick(Tile _tile, bool _overUI)
        {
            if (panelRoot.activeSelf && !_overUI && SelectedItemSpawner != null)
            {
                World.Instance.SpawnItem(SelectedItemSpawner.Item, SelectedItemSpawner.Quantity, _tile);
            }
        }
    }
}
using ColonyZ.Controllers.UI;
using ColonyZ.Models.Items;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using UnityEngine;

namespace ColonyZ.Controllers.Dev
{
    public class ItemsDevTool : MonoBehaviour
    {
        [SerializeField] private Transform entryContainer;

        [SerializeField] private GameObject itemSpawnerEntryPrefab;

        [SerializeField] private GameObject panelRoot;
        public ItemSpawnerEntry SelectedItemSpawner { get; set; }

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
                World.Instance.SpawnItem(SelectedItemSpawner.Item, SelectedItemSpawner.Quantity, _tile);
        }
    }
}
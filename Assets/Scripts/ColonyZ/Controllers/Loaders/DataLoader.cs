using ColonyZ.Models.Items;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Controllers.Loaders
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private Item[] itemData;
        [SerializeField] private TileObjectData[] objectData;
        [SerializeField] private SpriteData[] spriteData;

        public void Load()
        {
            foreach (var data in spriteData) data.Load();

            foreach (var data in objectData) TileObjectDataCache.Add(data);

            foreach (var data in itemData) ItemManager.RegisterItem(data);
        }
    }
}
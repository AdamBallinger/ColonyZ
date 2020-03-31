using Models.Items;
using Models.Map.Tiles.Objects;
using Models.Sprites;
using UnityEngine;

namespace Controllers.Loaders
{
    public class DataLoader : MonoBehaviour
    {
        [SerializeField] private SpriteData[] spriteData;
        [SerializeField] private TileObject[] objectData;
        [SerializeField] private Item[] itemData;

        public void Load()
        {
            foreach (var data in spriteData)
            {
                data.Load();
            }

            foreach (var data in objectData)
            {
                TileObjectCache.Add(data);
            }

            foreach (var data in itemData)
            {
                ItemManager.RegisterItem(data);
            }
        }
    }
}
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using UnityEngine;

namespace ColonyZ.Controllers.Loaders
{
    public class TileObjectsLoader : MonoBehaviour
    {
        [SerializeField] private TileObjectData[] tileObjects;

        public void Load()
        {
            foreach (var to in tileObjects) TileObjectDataCache.Add(to);
        }
    }
}
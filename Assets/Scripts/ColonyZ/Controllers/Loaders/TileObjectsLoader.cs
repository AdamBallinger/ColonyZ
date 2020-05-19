using ColonyZ.Models.Map.Tiles.Objects;
using UnityEngine;

namespace ColonyZ.Controllers.Loaders
{
    public class TileObjectsLoader : MonoBehaviour
    {
        [SerializeField] private TileObject[] tileObjects;

        public void Load()
        {
            foreach (var to in tileObjects) TileObjectCache.Add(to);
        }
    }
}
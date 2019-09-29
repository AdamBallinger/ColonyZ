using Models.Map.Tiles.Objects;
using UnityEngine;

namespace Controllers
{
    public class TileObjectsLoader : MonoBehaviour
    {
        [SerializeField]
        private TileObject[] tileObjects;
        
        public void Load()
        {
            foreach (var to in tileObjects)
            {
                TileObjectCache.Add(to);
            }
        }
    }
}

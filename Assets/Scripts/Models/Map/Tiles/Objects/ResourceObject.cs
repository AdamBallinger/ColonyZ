using Models.Items;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Resource_", menuName = "ColonyZ/Resource Object", order = 53)]
    public class ResourceObject : TileObject
    {
        public ResourceItem Item => item;

        public int Quantity => quanitity;
        
        [SerializeField]
        private ResourceItem item;

        [SerializeField]
        private int quanitity;
        
        public override bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }
    }
}
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Resource_", menuName = "ColonyZ/Resource Object", order = 53)]
    public class ResourceObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }
    }
}
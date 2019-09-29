using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Nature_", menuName = "ColonyZ/Nature Object", order = 53)]
    public class NatureObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }
    }
}
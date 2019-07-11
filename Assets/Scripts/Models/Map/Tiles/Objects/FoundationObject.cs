using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Foundation_", menuName = "ColonyZ/Foundation Object", order = 52)]
    public class FoundationObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return _tile?.Object == null;
        }
    }
}
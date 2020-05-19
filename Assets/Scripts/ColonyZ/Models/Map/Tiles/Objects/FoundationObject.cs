using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Foundation", menuName = "ColonyZ/Foundation Object", order = 52)]
    public class FoundationObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return _tile?.Object == null;
        }

        // Make foundations always appear below anything else.
        public override int GetSortingOrder()
        {
            return -10;
        }

        public override bool ConnectsWith(TileObject _other)
        {
            return _other.GetType() == typeof(FoundationObject);
        }
    }
}
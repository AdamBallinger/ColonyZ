using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Foundation", menuName = "ColonyZ/Foundation Object", order = 52)]
    public class FoundationObject : TileObject
    {
        public override bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }
        
        public override int GetSortingOrder()
        {
            // Make foundations always appear below anything else.
            return -10000;
        }

        public override bool ConnectsWith(TileObject _other)
        {
            return _other.GetType() == typeof(FoundationObject);
        }
    }
}
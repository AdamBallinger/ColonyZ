using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public class FoundationObject : TileObject
    {
        public FoundationObject(TileObjectData _data) : base(_data)
        {
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
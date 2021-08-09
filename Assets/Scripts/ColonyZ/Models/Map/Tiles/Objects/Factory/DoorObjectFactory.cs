using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects.Factory
{
    public class DoorObjectFactory : TileObjectFactory
    {
        public override TileObject GetObject(TileObjectData _data)
        {
            return new DoorObject(_data);
        }
    }
}
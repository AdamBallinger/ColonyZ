using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects.Factory
{
    public class FurnitureObjectFactory : TileObjectFactory
    {
        public override TileObject GetObject(TileObjectData _data)
        {
            return new FurnitureObject(_data);
        }
    }
}
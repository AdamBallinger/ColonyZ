using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects.Factory
{
    public class WallObjectFactory : TileObjectFactory
    {
        public override TileObject GetObject(TileObjectData _data)
        {
            return new WallObject(_data);
        }
    }
}
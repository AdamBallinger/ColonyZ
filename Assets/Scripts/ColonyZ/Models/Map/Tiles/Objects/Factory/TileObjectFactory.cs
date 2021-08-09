using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects.Factory
{
    public abstract class TileObjectFactory
    {
        public abstract TileObject GetObject(TileObjectData _data);
    }
}
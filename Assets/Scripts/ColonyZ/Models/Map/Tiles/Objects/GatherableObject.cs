using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public class GatherableObject : TileObject
    {
        public GatherableObject(TileObjectData _data) : base(_data)
        {
        }

        public override bool ConnectsWith(TileObject _other)
        {
            return ObjectData.SmartObject &&
                   string.CompareOrdinal(ObjectData.ObjectName, _other.ObjectData.ObjectName) == 0;
        }
    }
}
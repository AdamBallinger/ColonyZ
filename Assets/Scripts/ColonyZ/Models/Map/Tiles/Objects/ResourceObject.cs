using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public class ResourceObject : TileObject
    {
        public ResourceObject(TileObjectData _data) : base(_data)
        {
        }

        public override bool ConnectsWith(TileObject _other)
        {
            return ObjectData.SmartObject &&
                   string.CompareOrdinal(ObjectData.ObjectName, _other.ObjectData.ObjectName) == 0;
        }
    }
}
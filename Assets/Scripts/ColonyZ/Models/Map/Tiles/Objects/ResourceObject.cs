using ColonyZ.Models.Items;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Resource_", menuName = "ColonyZ/Resource Object", order = 53)]
    public class ResourceObject : TileObject
    {
        [SerializeField] private ResourceItem item;

        [SerializeField] private int quanitity;

        public ResourceItem Item => item;

        public int Quantity => quanitity;

        public override bool ConnectsWith(TileObject _other)
        {
            return string.CompareOrdinal(_other.ObjectName, ObjectName) == 0;
        }
    }
}
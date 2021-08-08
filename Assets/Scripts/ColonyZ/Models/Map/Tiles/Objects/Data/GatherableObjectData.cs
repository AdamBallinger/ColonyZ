using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects.Data
{
    [CreateAssetMenu(menuName = "ColonyZ/Gatherable Object Data", fileName = "Gatherable")]
    public class GatherableObjectData : TileObjectData
    {
        [SerializeField] private GatherMode gatherType;

        public GatherMode GatherType => gatherType;
    }
}
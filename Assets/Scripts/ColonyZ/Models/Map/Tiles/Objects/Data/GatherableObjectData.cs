using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects.Data
{
    [CreateAssetMenu(menuName = "ColonyZ/Gatherable Object Data", fileName = "Gatherable")]
    public class GatherableObjectData : TileObjectData
    {
        [SerializeField] private GatherMode gatherType;

        [SerializeField] private float gatherTime = 1.0f;

        public GatherMode GatherType => gatherType;

        /// <summary>
        ///     Time in seconds it takes to gather the object.
        /// </summary>
        public float GatherTime => gatherTime;
    }
}
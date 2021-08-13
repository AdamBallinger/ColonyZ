using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects.Data;

namespace ColonyZ.Models.AI.Jobs
{
    public class HarvestJob : Job
    {
        private GatherableObjectData gatherableData;
        
        /// <summary>
        ///     Create a new harvest job
        /// </summary>
        /// <param name="_targetTile"></param>
        public HarvestJob(Tile _targetTile) : base(_targetTile)
        {
            if (_targetTile.HasObject && _targetTile.Object.ObjectData is GatherableObjectData data)
            {
                gatherableData = data;
                JobName = $"{gatherableData.GatherType}: {gatherableData.ObjectName}";
            }
        }

        public override void OnComplete()
        {
            base.OnComplete();

            TargetTile.RemoveObject();
            // TODO: Spawn item from gatherable data.
        }
    }
}
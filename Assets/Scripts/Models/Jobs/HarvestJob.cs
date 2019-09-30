using Models.Map.Tiles;
using Models.Map.Tiles.Objects;

namespace Models.Jobs
{
    public class HarvestJob : Job
    {
        private ResourceObject resourceObject;
        
        /// <summary>
        /// Create a new harvest job
        /// </summary>
        /// <param name="_targetTile"></param>
        /// <param name="_harvestType">Type of harvest which names the job. E.g "Fell", "Mine"</param>
        public HarvestJob(Tile _targetTile, string _harvestType) : base(_targetTile)
        {
            JobName = $"{_harvestType}: {_targetTile.Object.ObjectName}";
            resourceObject = _targetTile.Object as ResourceObject;
        }

        public override void OnComplete()
        {
            base.OnComplete();
            
            TargetTile.RemoveObject();
            // TODO: Decide how to spawn harvest items..
        }
    }
}
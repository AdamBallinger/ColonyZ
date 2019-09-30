using Models.Map.Tiles;

namespace Models.Jobs
{
    public class HarvestJob : Job
    {
        /// <summary>
        /// Create a new harvest job
        /// </summary>
        /// <param name="_targetTile"></param>
        /// <param name="_harvestType">Type of harvest which names the job. E.g "Fell", "Mine"</param>
        public HarvestJob(Tile _targetTile, string _harvestType) : base(_targetTile)
        {
            JobName = $"{_harvestType}: {_targetTile.Object.ObjectName}";
        }

        public override void OnComplete()
        {
            base.OnComplete();
            
            TargetTile.RemoveObject();
            // TODO: Decide how to spawn harvest items..
        }
    }
}
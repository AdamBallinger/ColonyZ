using Models.Map.Tiles;

namespace Models.Jobs
{
    public class HarvestJob : Job
    {
        public HarvestJob(Tile _targetTile) : base(_targetTile)
        {
            JobName = "Harvest: " + _targetTile.Object.ObjectName;
        }

        public override void OnComplete()
        {
            base.OnComplete();
            
            TargetTile.RemoveObject();
            // TODO: Decide how to spawn harvest items..
        }
    }
}
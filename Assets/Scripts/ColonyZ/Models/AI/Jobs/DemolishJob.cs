using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.AI.Jobs
{
    public class DemolishJob : Job
    {
        public DemolishJob(Tile _targetTile) : base(_targetTile)
        {
            JobName = "Demolish: " + _targetTile.Object.ObjectName;
        }

        public override void OnComplete()
        {
            base.OnComplete();

            TargetTile.RemoveObject();
        }
    }
}
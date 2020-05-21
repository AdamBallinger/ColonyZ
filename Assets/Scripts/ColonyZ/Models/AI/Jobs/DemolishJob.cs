using ColonyZ.Models.Map.Tiles;
using Newtonsoft.Json.Linq;

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

        public override void OnLoad(JToken _dataToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
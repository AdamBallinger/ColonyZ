using System;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.AI.Jobs
{
    public class HarvestJob : Job
    {
        private ResourceObject resourceObject;

        /// <summary>
        ///     Create a new harvest job
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
            World.Instance.SpawnItem(resourceObject.Item, resourceObject.Quantity, TargetTile);
        }

        public override void OnLoad(JToken _dataToken)
        {
            throw new NotImplementedException();
        }
    }
}
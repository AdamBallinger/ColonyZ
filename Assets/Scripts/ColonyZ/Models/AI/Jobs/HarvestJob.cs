using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.AI.Jobs
{
    public class HarvestJob : Job
    {
        private ResourceObject resourceObject;

        private string harvestType;

        /// <summary>
        ///     Create a new harvest job
        /// </summary>
        /// <param name="_targetTile"></param>
        /// <param name="_harvestType">Type of harvest which names the job. E.g "Fell", "Mine"</param>
        public HarvestJob(Tile _targetTile, string _harvestType) : base(_targetTile)
        {
            if (_targetTile.HasObject)
            {
                JobName = $"{_harvestType}: {_targetTile.Object.ObjectName}";
                resourceObject = _targetTile.Object as ResourceObject;
                harvestType = _harvestType;
            }
            else
            {
                Debug.LogError($"[HarvestJob] Tile index: {World.Instance.GetTileIndex(_targetTile)}" +
                               " has no object to harvest! Save file was modified and is not supported!");
            }
        }

        /// <summary>
        /// Used for loading only.
        /// </summary>
        /// <param name="_targetTile"></param>
        public HarvestJob(Tile _targetTile) : this(_targetTile, string.Empty)
        {
        }

        public override void OnComplete()
        {
            base.OnComplete();

            TargetTile.RemoveObject();
            //World.Instance.SpawnItem(resourceObject.Item, resourceObject.Quantity, TargetTile);
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);
            _writer.WriteProperty("harvest_type", harvestType);
        }

        public override void OnLoad(JToken _dataToken)
        {
            base.OnLoad(_dataToken);

            harvestType = _dataToken["harvest_type"].Value<string>();
            JobName = $"{harvestType}: {TargetTile.Object.ObjectName}";
        }
    }
}
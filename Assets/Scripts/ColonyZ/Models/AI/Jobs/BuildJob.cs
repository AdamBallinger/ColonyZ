using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Saving;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.AI.Jobs
{
    public class BuildJob : Job
    {
        private TileObject tileObject;

        public BuildJob(Tile _targetTile, TileObject _object) : base(_targetTile)
        {
            JobName = "Build: " + _object.ObjectName;
            tileObject = _object;
        }

        /// <summary>
        /// Used only for loading.
        /// </summary>
        /// <param name="_targetTile"></param>
        public BuildJob(Tile _targetTile) : base(_targetTile)
        {
        }

        public override void OnComplete()
        {
            base.OnComplete();

            TargetTile.SetObject(tileObject);
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);

            _writer.WriteProperty("object_name", tileObject.ObjectName);
        }

        public override void OnLoad(JToken _dataToken)
        {
            tileObject = TileObjectCache.GetObject(_dataToken["object_name"].Value<string>());
            JobName = "Build: " + tileObject.ObjectName;
        }
    }
}
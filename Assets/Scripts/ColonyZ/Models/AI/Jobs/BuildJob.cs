using System;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Saving;
using ColonyZ.Utils;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.AI.Jobs
{
    public class BuildJob : Job
    {
        private TileObject tileObject;

        public BuildJob(Tile _targetTile, TileObject _object) : base(_targetTile)
        {
            JobName = "Build: " + _object.ObjectData.ObjectName;
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

        public override void OnCancelled()
        {
            var w = ObjectRotationUtil.GetRotatedObjectWidth(tileObject.ObjectData, tileObject.ObjectRotation);
            var h = ObjectRotationUtil.GetRotatedObjectHeight(tileObject.ObjectData, tileObject.ObjectRotation);
            
            for (var xOff = 0; xOff < w; xOff++)
            for (var yOff = 0; yOff < h; yOff++)
            {
                var t = World.Instance.GetTileAt(TargetTile.X + xOff, TargetTile.Y - yOff);
                t.RemoveObject();
            }
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);

            _writer.WriteProperty("object_name", tileObject.ObjectData.ObjectName);
            _writer.WriteProperty("object_rotation", tileObject.ObjectRotation);
        }

        public override void OnLoad(JToken _dataToken)
        {
            base.OnLoad(_dataToken);
            
            var objectData = TileObjectDataCache.GetData(_dataToken["object_name"].Value<string>());
            tileObject = ObjectFactoryUtil.GetFactory(objectData.FactoryType).GetObject(objectData);
            Enum.TryParse<ObjectRotation>(_dataToken["object_rotation"].Value<string>(), out var rotation);
            tileObject.ObjectRotation = rotation;
            JobName = "Build: " + tileObject.ObjectData.ObjectName;
        }
    }
}
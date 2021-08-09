using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Saving;
using ColonyZ.Models.UI.Context;
using ColonyZ.Utils;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public class FoundationObject : TileObject
    {
        /// <summary>
        ///     Object data for the object that is being built on this foundation.
        /// </summary>
        private TileObjectData Object { get; set; }
        
        private ObjectRotation Rotation { get; }
        
        private Tile Origin { get; set; }

        public FoundationObject(TileObjectData _data) : base(_data)
        {
        }

        public FoundationObject(TileObjectData _data,
            TileObjectData _other,
            ObjectRotation _rotation,
            Tile _originTile) : base(_data)
        {
            Object = _other;
            Rotation = _rotation;
            Origin = _originTile;
        }

        public void PlaceFullFoundation()
        {
            var w = ObjectRotationUtil.GetRotatedObjectWidth(Object, Rotation);
            var h = ObjectRotationUtil.GetRotatedObjectHeight(Object, Rotation);
            
            for (var xOff = 0; xOff < w; xOff++)
            for (var yOff = 0; yOff < h; yOff++)
            {
                var t = World.Instance.GetTileAt(OriginTile.X + xOff, OriginTile.Y - yOff);
                var f = new FoundationObject(ObjectData, Object, ObjectRotation, Origin);
                t.SetObject(f);
            }
        }
        
        public override int GetSortingOrder()
        {
            // Make foundations always appear below anything else.
            return -10000;
        }

        public override bool ConnectsWith(TileObject _other)
        {
            if (_other is FoundationObject other)
            {
                if (Object.MultiTile)
                {
                    return other.Origin == Origin;
                }
                
                return other.Object == Object;
            }

            return false;
        }

        public override string GetSelectionName()
        {
            return Object.ObjectName;
        }

        public override ContextAction[] GetContextActions()
        {
            return new[]
            {
                new ContextAction("Cancel", () => JobManager.Instance.CancelJob(Origin.CurrentJob))
            };
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);
            
            _writer.WriteProperty("object_name", Object.ObjectName);
            _writer.WriteProperty("origin", World.Instance.GetTileIndex(Origin));
        }

        public override void OnLoad(JToken _dataToken)
        {
            Object = TileObjectDataCache.GetData(_dataToken["object_name"].Value<string>());
            Origin = World.Instance.GetTileAt(_dataToken["origin"].Value<int>());
            base.OnLoad(_dataToken);
        }
    }
}
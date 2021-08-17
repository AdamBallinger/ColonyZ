using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Map.Zones
{
    public abstract class Zone : ISaveable
    {
        public string ZoneName { get; protected set; }

        /// <summary>
        ///     Determines if the zone can contain objects when being placed in the word.
        ///     This does not affect the zone when an object is being built after the zone is placed.
        /// </summary>
        public bool CanContainObjects { get; protected set; }

        public Vector2Int MinimumSize { get; protected set; }

        /// <summary>
        ///     Bottom left corner of the zone.
        /// </summary>
        public Vector2Int Origin { get; protected set; }

        public Vector2Int Size { get; protected set; }

        protected Zone()
        {
            MinimumSize = Vector2Int.one;
        }

        public virtual void SetOrigin(int _x, int _y)
        {
            Origin = new Vector2Int(_x, _y);
        }

        protected void SetOrigin(Tile _tile)
        {
            SetOrigin(_tile.X, _tile.Y);
        }

        public virtual void SetSize(int _width, int _height)
        {
            Size = new Vector2Int(_width, _height);
        }

        public virtual bool CanPlace(Tile _tile)
        {
            if (_tile.Zone != null) return false;
            if (_tile.HasObject && !CanContainObjects) return false;

            return true;
        }

        public bool CanSave()
        {
            return true;
        }

        public void OnSave(SaveGameWriter _writer)
        {
            var originTile = World.Instance.GetTileAt(Origin);
            _writer.WriteProperty("t_index", World.Instance.GetTileIndex(originTile));
            _writer.WriteProperty("zone_type", GetType().FullName);
            _writer.WriteProperty("size_x", Size.x);
            _writer.WriteProperty("size_y", Size.y);
        }

        public abstract void OnLoad(JToken _dataToken);
    }
}
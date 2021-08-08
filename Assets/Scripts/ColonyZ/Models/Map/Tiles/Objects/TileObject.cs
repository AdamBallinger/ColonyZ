using System;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Saving;
using ColonyZ.Models.UI;
using ColonyZ.Models.UI.Context;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public abstract class TileObject : ISelectable, ISaveable, IContextProvider
    {
        /// <summary>
        ///     Data for this object.
        /// </summary>
        public TileObjectData ObjectData { get; }
        
        /// <summary>
        ///     The Tile this object originates from. If the object is a multi tile object, then this is the "base" tile
        ///     for that object.
        /// </summary>
        public Tile OriginTile { get; set; }

        /// <summary>
        ///     The Tile this part of a object occupies. If the object is a single type, then this will be the same as OriginTile.
        /// </summary>
        public Tile Tile { get; set; }

        public ObjectRotation ObjectRotation { get; set; }

        /// <summary>
        ///     Flag used to only save the origin tile for multi tile objects.
        /// </summary>
        private bool shouldSave = true;

        public TileObject(TileObjectData _data)
        {
            ObjectData = _data;
        }

        public virtual void Update()
        {
        }

        public virtual bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }

        /// <summary>
        ///     Returns if this object connects to a given object. By default, no objects connect to each other.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public virtual bool ConnectsWith(TileObject _other)
        {
            return false;
        }

        /// <summary>
        ///     Returns the sprite index to use for single tile structures. This does nothing for dynamic tile objects.
        ///     This will consider object rotation, if the object is rotatable.
        /// </summary>
        /// <returns></returns>
        public virtual int GetSpriteIndex()
        {
            return ObjectData.Rotatable ? (int)ObjectRotation : 0;
        }

        public virtual int GetSortingOrder()
        {
            return World.Instance.Height - Tile.Y;
        }

        #region ISelectable Implementation

        public Sprite GetSelectionIcon()
        {
            return ObjectData.GetIcon();
        }

        public string GetSelectionName()
        {
            return ObjectData.ObjectName;
        }

        public string GetSelectionDescription()
        {
            return Tile.GetSelectionDescription() +
                   "Durability: 0/0\n";
        }

        public Vector2 GetPosition()
        {
            return OriginTile.Position;
        }

        #endregion

        public bool CanSave()
        {
            return shouldSave;
        }

        public void OnSave(SaveGameWriter _writer)
        {
            _writer.WriteProperty("data_name", ObjectData.ObjectName);
            _writer.WriteProperty("t_index", World.Instance.GetTileIndex(OriginTile));
            _writer.WriteProperty("rot", (int)ObjectRotation);

            // Only save the origin tile for multi tile objects.
            if (ObjectData.MultiTile) shouldSave = false;
        }

        public void OnLoad(JToken _dataToken)
        {
            var tileIndex = _dataToken["t_index"].Value<int>();
            Enum.TryParse<ObjectRotation>(_dataToken["rot"].Value<int>().ToString(), true, out var rot);
            ObjectRotation = rot;

            World.Instance.GetTileAt(tileIndex).SetObject(this, false);
        }

        public ContextAction[] GetContextActions()
        {
            return new[]
            {
                new ContextAction("Remove", () =>
                {
                    if (World.Instance.WorldActionProcessor.GodMode) Tile.RemoveObject();
                    else JobManager.Instance.AddJob(new DemolishJob(Tile));
                })
            };
        }

        public string GetContextMenuName()
        {
            return ObjectData.ObjectName;
        }
    }
}
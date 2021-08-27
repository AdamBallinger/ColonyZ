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

        public ObjectRotation ObjectRotation { get; set; }

        /// <summary>
        ///     Flag used to only save the origin tile for multi tile objects.
        /// </summary>
        protected bool shouldSave = true;

        protected TileObject(TileObjectData _data)
        {
            ObjectData = _data;
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
            return World.Instance.Height - OriginTile.Y;
        }

        #region ISelectable Implementation

        public virtual string GetSelectionName()
        {
            return ObjectData.ObjectName;
        }

        public virtual string GetSelectionDescription()
        {
            return "Condition: 100%\n";
        }

        public virtual Vector2 GetPosition()
        {
            return OriginTile.Position;
        }

        #endregion

        public virtual bool CanSave()
        {
            return shouldSave;
        }

        public virtual void OnSave(SaveGameWriter _writer)
        {
            _writer.WriteProperty("data_name", ObjectData.ObjectName);
            _writer.WriteProperty("t_index", World.Instance.GetTileIndex(OriginTile));
            _writer.WriteProperty("rot", (int)ObjectRotation);

            // Only save the origin tile for multi tile objects.
            if (ObjectData.MultiTile) shouldSave = false;
        }

        public virtual void OnLoad(JToken _dataToken)
        {
            var tileIndex = _dataToken["t_index"].Value<int>();
            Enum.TryParse<ObjectRotation>(_dataToken["rot"].Value<int>().ToString(), true, out var rot);
            ObjectRotation = rot;

            World.Instance.GetTileAt(tileIndex).SetObject(this, false);
        }

        public virtual ContextAction[] GetContextActions()
        {
            return new[]
            {
                new ContextAction("Remove", () =>
                {
                    if (World.Instance.WorldActionProcessor.GodMode) OriginTile.RemoveObject();
                    else
                    {
                        if (ObjectData.Buildable) JobManager.Instance.AddJob(new DemolishJob(OriginTile));
                        else if (this is GatherableObject)
                        {
                            JobManager.Instance.AddJob(new HarvestJob(OriginTile));
                        }
                    };
                })
            };
        }

        public virtual string GetContextMenuName()
        {
            return ObjectData.ObjectName;
        }
    }
}
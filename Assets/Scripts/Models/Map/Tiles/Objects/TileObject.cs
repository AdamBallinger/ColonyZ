using System.Collections.Generic;
using Controllers;
using Models.Sprites;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public abstract class TileObject
    {
        /// <summary>
        /// The Tile this object originates from. If the object is a multi tile object, then this is the "base" tile
        /// for that object.
        /// </summary>
        public Tile OriginTile { get; set; }

        /// <summary>
        /// The Tile this part of a object occupies. If the object is a single type, then this will be the same as OriginTile.
        /// If the object is a multi tile object, then it will point to the tile each part of the object is placed on.
        /// </summary>
        public Tile Tile { get; set; }

        /// <summary>
        /// The type of this object (Single or multi tile).
        /// </summary>
        public TileObjectType Type { get; protected set; }

        public SpriteData SpriteData { get; protected set; }

        /// <summary>
        /// Name of the object. This refers to the name associated with this object in the TileStructureRegistry.
        /// </summary>
        public string ObjectName { get; protected set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        
        public TileEnterability Enterability { get; protected set; }
        
        public float MovementModifier { get; protected set; }

        /// <summary>
        /// Returns whether this object occupies more than 1 tile.
        /// </summary>
        public bool MultiTile => Width > 1 || Height > 1;

        protected bool ConnectsToSelf { get; set; }

        protected List<string> Connectables { get; set; }

        protected TileObject(string _objectName)
        {
            ObjectName = _objectName;
            Type = TileObjectType.Single_Tile;
            Width = 1;
            Height = 1;
            Enterability = TileEnterability.None;
            MovementModifier = 0.0f;
            SpriteData = SpriteDataController.GetSpriteData(ObjectName);
            ConnectsToSelf = false;
            Connectables = new List<string>();
        }

        protected void CopyInto(TileObject _clone)
        {
            _clone.SpriteData = SpriteData;
            _clone.Type = Type;
            _clone.ObjectName = ObjectName;
            _clone.Width = Width;
            _clone.Height = Height;
            _clone.Enterability = Enterability;
            _clone.MovementModifier = MovementModifier;
            _clone.ConnectsToSelf = ConnectsToSelf;
            _clone.Connectables = Connectables;
        }

        public abstract TileObject Clone();

        /// <summary>
        /// Returns if this structure connects to a given structure.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public bool ConnectsWith(TileObject _other)
        {
            if(_other == null)
            {
                return false;
            }

            return ConnectsToSelf && _other.ObjectName.Equals(ObjectName) || Connectables.Contains(_other.ObjectName);
        }

        /// <summary>
        /// Returns the sprite index to use for single tile structures.
        /// </summary>
        /// <returns></returns>
        public virtual int GetSpriteIndex()
        {
            return 0;
        }

        /// <summary>
        /// Checks if the structure can be placed on the given tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public abstract bool CanPlace(Tile _tile);

        /// <summary>
        /// Return an icon sprite for this tile structure. This is used for display the structure in UI.
        /// </summary>
        /// <returns></returns>
        public abstract Sprite GetIcon();
    }
}

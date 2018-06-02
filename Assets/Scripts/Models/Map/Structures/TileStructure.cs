using System.Collections.Generic;
using Controllers;
using Models.Sprites;
using UnityEngine;

namespace Models.Map.Structures
{
    public abstract class TileStructure
    {
        /// <summary>
        /// The Tile this structure originates from. If the structure is a multi tile structure, then this is the "base" tile
        /// for that structure.
        /// </summary>
        public Tile OriginTile { get; set; }

        /// <summary>
        /// The Tile this part of a structure occupies. If the struction is a single type, then this will be the same as OriginTile.
        /// If the structure is a multi tile structure, then it will point to the tile each part of the structure is placed on.
        /// </summary>
        public Tile Tile { get; set; }

        /// <summary>
        /// The type of this structure (Single or multi tile).
        /// </summary>
        public TileStructureType Type { get; protected set; }

        public SpriteData SpriteData { get; protected set; }

        /// <summary>
        /// Name of the structure. This refers to the name associated with this structure in the TileStructureRegistry.
        /// </summary>
        public string StructureName { get; protected set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        /// <summary>
        /// Returns whether this structure occupies more than 1 tile.
        /// </summary>
        public bool MultiTile => Width > 1 || Height > 1;

        protected bool ConnectsToSelf { get; set; }

        protected List<string> Connectables { get; set; }

        protected TileStructure(string _structureName)
        {
            StructureName = _structureName;
            Type = TileStructureType.Single_Tile;
            Width = 1;
            Height = 1;
            SpriteData = SpriteDataController.GetSpriteData(StructureName);
            ConnectsToSelf = false;
            Connectables = new List<string>();
        }

        protected void CopyInto(TileStructure _clone)
        {
            _clone.SpriteData = SpriteData;
            _clone.Type = Type;
            _clone.StructureName = StructureName;
            _clone.Width = Width;
            _clone.Height = Height;
            _clone.ConnectsToSelf = ConnectsToSelf;
            _clone.Connectables = Connectables;
        }

        public abstract TileStructure Clone();

        /// <summary>
        /// Returns if this structure connects to a given structure.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public bool ConnectsWith(TileStructure _other)
        {
            if(_other == null)
            {
                return false;
            }

            return ConnectsToSelf && _other.StructureName.Equals(StructureName) || Connectables.Contains(_other.StructureName);
        }

        /// <summary>
        /// Return an icon sprite for this tile structure. This is used for display the structure in UI.
        /// </summary>
        /// <returns></returns>
        public abstract Sprite GetIcon();
    }
}

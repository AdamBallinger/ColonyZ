using System.Collections.Generic;
using Controllers;
using Models.Sprites;

namespace Models.Map
{
    public class TileStructure
    {
        /// <summary>
        /// The Tile this structure originates from. If the structure is a multi tile structure, then this is the "base" tile
        /// for that structure.
        /// </summary>
        public Tile OriginTile { get; set; }

        public TileSpriteData SpriteData { get; }

        /// <summary>
        /// The type of this structure (Single or multi tile).
        /// </summary>
        public TileStructureType Type { get; protected set; }

        /// <summary>
        /// Name of the structure. This refers to the name associated with this structure in the TileStructureRegistry.
        /// </summary>
        public string StructureName { get; }

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
            SpriteData = SpriteDataController.GetSpriteDataFor<TileSpriteData>(StructureName);
            ConnectsToSelf = false;
            Connectables = new List<string>();
        }

        private TileStructure(TileStructure _source)
        {
            SpriteData = _source.SpriteData;
            Type = _source.Type;
            StructureName = _source.StructureName;
            Width = _source.Width;
            Height = _source.Height;
            ConnectsToSelf = _source.ConnectsToSelf;
            Connectables = _source.Connectables;
        }

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

            return ConnectsToSelf && _other.StructureName.Equals(StructureName);
        }

        public virtual TileStructure Clone()
        {
            return new TileStructure(this);
        }
    }
}

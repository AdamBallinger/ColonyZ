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

        // Type of structure Wall, machine etc.
        public TileStructureType Type { get; protected set; }

        public string StructureName { get; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        protected TileStructure(string _structureName)
        {
            StructureName = _structureName;
            Type = TileStructureType.Single_Tile;
            Width = 1;
            Height = 1;
            SpriteData = SpriteDataController.GetSpriteDataFor<TileSpriteData>(StructureName);
        }

        private TileStructure(TileStructure _source)
        {
            SpriteData = _source.SpriteData;
            Type = _source.Type;
            StructureName = _source.StructureName;
            Width = _source.Width;
            Height = _source.Height;
        }

        public virtual TileStructure Clone()
        {
            return new TileStructure(this);
        }
    }
}

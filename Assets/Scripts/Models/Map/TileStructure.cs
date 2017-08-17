using Controllers;

namespace Models.Map
{
    public class TileStructure
    {

        public Tile OriginTile { get; set; }

        public TileSpriteData SpriteData { get; }

        // Type of structure Wall, machine etc.
        public TileStructureType Type { get; }

        public string StructureName { get; }

        public uint Width { get; }
        public uint Height { get; }

        public TileStructure(uint _width, uint _height, string _structureName, TileStructureType _type)
        {
            Width = _width;
            Height = _height;
            StructureName = _structureName;
            Type = _type;
            SpriteData = SpriteDataController.GetSpriteDataFor<TileSpriteData>(_structureName);
        }
    }
}

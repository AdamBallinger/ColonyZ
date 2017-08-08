namespace Models.Map
{
	public class TileStructure 
	{

        public Tile OriginTile { get; set; }

	    public TileSpriteData SpriteData { get; }

        // Type of structure Wall, machine etc.
        public TileStructureType Type { get; }

        public string StructureName { get; private set; }

        public uint Width { get; }
	    public uint Height { get; }

        public TileStructure(uint _width, uint _height, string _structureName, TileStructureType _type, TileSpriteData _spriteData)
        {
            Width = _width;
            Height = _height;
            Type = _type;
            SpriteData = _spriteData;
            StructureName = _structureName;
        }
	}
}

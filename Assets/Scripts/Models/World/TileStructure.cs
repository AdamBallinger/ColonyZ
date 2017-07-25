namespace Models.World
{
	public class TileStructure 
	{

        public Tile OriginTile { get; set; }

	    public TileSpriteData SpriteData { get; }

        // Type of structure Wall, machine etc.
        public TileStructureType Type { get; }

        public uint Width { get; }
	    public uint Height { get; }

        public TileStructure(uint _width, uint _height, TileStructureType _type, TileSpriteData _spriteData)
        {
            Width = _width;
            Height = _height;
            Type = _type;
            SpriteData = _spriteData;
        }
	}
}

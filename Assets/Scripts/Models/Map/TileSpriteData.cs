using Models.Sprites;

namespace Models.Map
{
	public class TileSpriteData : ISpriteData
	{
        /// <summary>
        /// Is the sprite data for the tile a single sprite or a set of tiles.
        /// </summary>
	    private bool IsTileSet { get; }

        /// <summary>
        /// The name of the sprite associated with this tile data's tile.
        /// If marked as a tileset, additonal data can be appended.
        /// </summary>
        private string SpriteName { get; }

        /// <summary>
        /// Sprite path in the Resources directory.
        /// </summary>
        private string ResourceLocation { get; }

        private string MappedObjectName { get; }

	    /// <summary>
	    /// Creates a new tile sprite data mapped to the given tile name.
	    /// </summary>
	    /// <param name="_tileName"></param>
	    /// <param name="_tileset"></param>
	    /// <param name="_spriteBaseName"></param>
	    /// <param name="_resLocation"></param>
	    public TileSpriteData(string _tileName, bool _tileset, string _spriteBaseName, string _resLocation)
        {
            MappedObjectName = _tileName;
            IsTileSet = _tileset;
            SpriteName = _spriteBaseName;
            ResourceLocation = _resLocation;
        }

	    public bool GetIsTileSet()
	    {
	        return IsTileSet;
	    }

	    public string GetSpriteName()
	    {
	        return SpriteName;
	    }

	    public string GetResourcesPath()
	    {
	        return ResourceLocation;
	    }

	    public string GetMappedObjectName()
	    {
	        return MappedObjectName;
	    }
	}
}

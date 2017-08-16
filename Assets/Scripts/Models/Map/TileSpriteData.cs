using System;
using Models.Sprites;
using Newtonsoft.Json;

namespace Models.Map
{
    [Serializable]
	public class TileSpriteData : ISpriteData
	{
        /// <summary>
        /// Is the sprite data for the tile a single sprite or a set of tiles.
        /// </summary>
        [JsonProperty]
	    private bool IsTileSet { get; set; }

        /// <summary>
        /// The name of the sprite associated with this tile data's tile.
        /// If marked as a tileset, additonal data can be appended.
        /// </summary>
        [JsonProperty]
        private string SpriteName { get; set; }

        /// <summary>
        /// Sprite path in the Resources directory.
        /// </summary>
        [JsonProperty]
        private string ResourceLocation { get; set; }

	    [JsonProperty]
        private string MappedObjectName { get; set; }

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

using System;
using Newtonsoft.Json;

namespace Models.Sprites
{
    [Serializable]
	public class EntitySpriteData : ISpriteData
	{	
        /// <summary>
        /// If an entity sprite data is marked as dynamic, then there are multiple sprite variants to the given entity.
        /// These variants can include anything such as directional based sprites for characters, or item stack icons
        /// based on the quantity in the stack.
        /// </summary>
        [JsonProperty]
        private bool IsDynamic { get; set; }

        /// <summary>
        /// The sprite name allocated to the entity of this sprite data.
        /// If the data is marked as dynamic, then additional data can be appended to the sprite name.
        /// </summary>
        [JsonProperty]
        private string SpriteName { get; set; }

        /// <summary>
        /// The path for the sprite in the Resources directory.
        /// </summary>
        [JsonProperty]
        private string ResourceLocation { get; set; }

        [JsonProperty]
        private string MappedObjectName { get; set; }

        public EntitySpriteData(string _entityName, bool _isDynamic, string _spriteBaseName, string _resLocation)
        {
            MappedObjectName = _entityName;
            IsDynamic = _isDynamic;
            SpriteName = _spriteBaseName;
            ResourceLocation = _resLocation;
        }

	    public bool GetIsTileSet()
	    {
	        return IsDynamic;
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

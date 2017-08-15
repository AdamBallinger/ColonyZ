using Models.Sprites;

namespace Models.Entities
{
	public class EntitySpriteData : ISpriteData
	{	
        /// <summary>
        /// If an entity sprite data is marked as dynamic, then there are multiple sprite variants to the given entity.
        /// These variants can include anything such as directional based sprites for characters, or item stack icons
        /// based on the quantity in the stack.
        /// </summary>
        private bool IsDynamic { get; }

        /// <summary>
        /// The sprite name allocated to the entity of this sprite data.
        /// If the data is marked as dynamic, then additional data can be appended to the sprite name.
        /// </summary>
        private string SpriteName { get; }

        /// <summary>
        /// The path for the sprite in the Resources directory.
        /// </summary>
        private string ResourceLocation { get; }

        private string MappedObjectName { get; }


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

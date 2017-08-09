namespace Models.Entities
{
	public struct EntitySpriteData 
	{	
        /// <summary>
        /// If an entity sprite data is marked as dynamic, then there are multiple sprite variants to the given entity.
        /// These variants can include anything such as directional based sprites for characters, or item stack icons
        /// based on the quantity in the stack.
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// The sprite name allocated to the entity of this sprite data.
        /// If the data is marked as dynamic, then additional data can be appended to the sprite name.
        /// </summary>
        public string SpriteName { get; set; }

        /// <summary>
        /// The path for the sprite in the Resources directory.
        /// </summary>
        public string ResourceLocation { get; set; }
	}
}

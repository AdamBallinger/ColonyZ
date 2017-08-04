namespace Models.World
{
	public struct TileSpriteData
	{
        /// <summary>
        /// Is the sprite data for the tile a single sprite or a set of tiles.
        /// </summary>
	    public bool IsTileSet { get; set; }

        /// <summary>
        /// The name of the sprite associated with this tile data's tile.
        /// If marked as a tileset, additonal data can be appended.
        /// </summary>
        public string SpriteName { get; set; }

        /// <summary>
        /// Sprite path in the Resources directory.
        /// </summary>
        public string ResourceLocation { get; set; }
	}
}

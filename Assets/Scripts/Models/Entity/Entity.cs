using System;
using Models.World;

namespace Models.Entity
{
	public abstract class Entity 
	{
        /// <summary>
        /// Precise X coordinate of entity.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Precise Y coordinate of entity.
        /// </summary>
        public float Y { get; }

        /// <summary>
        /// The current tile the pivot of the entity is placed within.
        /// </summary>
        public Tile CurrentTile { get; }

        /// <summary>
        /// Sprite data for this entity.
        /// </summary>
        public EntitySpriteData SpriteData { get; set; }

        protected Entity(Tile _tile)
        {
            X = _tile.X;
            Y = _tile.Y;
            CurrentTile = _tile;
        }
	}
}

using Models.Map;
using Models.Sprites;
using UnityEngine;

namespace Models.Entities
{
    public abstract class Entity
    {
        /// <summary>
        /// Precise X coordinate of entity.
        /// </summary>
        public float X => CurrentTile.X + TileOffset.x;

        /// <summary>
        /// Precise Y coordinate of entity.
        /// </summary>
        public float Y => CurrentTile.Y + TileOffset.y;

        /// <summary>
        /// Current entity health.
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Entity's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current tile the pivot of the entity is placed within.
        /// </summary>
        public Tile CurrentTile { get; set; }

        public Vector2 TileOffset { get; set; }

        /// <summary>
        /// Sprite data for this entity.
        /// </summary>
        public EntitySpriteData SpriteData { get; set; }

        protected Entity(Tile _tile)
        {
            CurrentTile = _tile;
            TileOffset = Vector2.zero;
        }

        public void HealEntity(int _damage)
        {
            Health += _damage;
        }

        public void DamageEntity(int _damage)
        {
            Health -= _damage;
        }

        public abstract void Update();
    }
}

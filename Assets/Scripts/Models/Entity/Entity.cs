using System;
using Models.Map;
using UnityEngine;

namespace Models.Entity
{
	public abstract class Entity 
	{
        /// <summary>
        /// Precise X coordinate of entity.
        /// </summary>
        public float X { get; protected set; }

        /// <summary>
        /// Precise Y coordinate of entity.
        /// </summary>
        public float Y { get; protected set; }

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
	    public Tile CurrentTile => World.Instance.GetTileAt(X, Y);

	    /// <summary>
        /// Sprite data for this entity.
        /// </summary>
        public EntitySpriteData SpriteData { get; set; }

        protected Entity(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        /// <summary>
        /// Set the position of the entity in the world. Position is clamped within the bounds of the world.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public void SetPosition(float _x, float _y)
        {
            X = Mathf.Clamp(_x, 0, World.Instance.Width - 1);
            Y = Mathf.Clamp(_y, 0, World.Instance.Height - 1);
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

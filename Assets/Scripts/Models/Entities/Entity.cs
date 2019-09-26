using Models.Map.Tiles;
using Models.Sprites;
using Models.UI;
using UnityEngine;

namespace Models.Entities
{
    public abstract class Entity : ISelectable
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
        public SpriteData SpriteData { get; set; }

        protected Entity(Tile _tile)
        {
            CurrentTile = _tile;
            TileOffset = Vector2.zero;
        }

        public abstract void Update();
        
        public Sprite GetSelectionIcon()
        {
            return null;
        }

        public string GetSelectionName()
        {
            return Name;
        }

        public virtual string GetSelectionDescription()
        {
            return CurrentTile.GetSelectionDescription();
        }
    }
}

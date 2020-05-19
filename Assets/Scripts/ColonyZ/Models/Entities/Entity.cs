using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.UI;
using UnityEngine;

namespace ColonyZ.Models.Entities
{
    public abstract class Entity : ISelectable
    {
        protected Entity(Tile _tile)
        {
            CurrentTile = _tile;
            TileOffset = Vector2.zero;
        }

        /// <summary>
        ///     Precise X coordinate of entity.
        /// </summary>
        public float X => CurrentTile.X + TileOffset.x;

        /// <summary>
        ///     Precise Y coordinate of entity.
        /// </summary>
        public float Y => CurrentTile.Y + TileOffset.y;

        /// <summary>
        ///     Entity's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The current tile the pivot of the entity is placed within.
        /// </summary>
        public Tile CurrentTile { get; set; }

        public Vector2 TileOffset { get; set; }

        public abstract Sprite GetSelectionIcon();

        public string GetSelectionName()
        {
            return Name;
        }

        public virtual string GetSelectionDescription()
        {
            return $"Position: ({X:0.#}, {Y:0.#})\n";
        }

        public Vector2 GetPosition()
        {
            return new Vector2(X, Y);
        }

        public virtual int GetSortingOrder()
        {
            return World.Instance.Height - CurrentTile.Y;
        }

        public abstract void Update();
    }
}
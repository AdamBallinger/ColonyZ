using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Saving;
using ColonyZ.Models.UI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Entities
{
    public abstract class Entity : ISelectable, ISaveable
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

        public virtual bool CanSave()
        {
            return true;
        }

        public virtual void OnSave(SaveGameWriter _writer)
        {
            _writer.WriteProperty("entity_name", Name);
            _writer.WriteProperty("t_index", World.Instance.GetTileIndex(CurrentTile));
        }

        public virtual void OnLoad(JToken _dataToken)
        {
            var tileIndex = _dataToken["t_index"].Value<int>();
            CurrentTile = World.Instance.GetTileAt(tileIndex);
        }
    }
}
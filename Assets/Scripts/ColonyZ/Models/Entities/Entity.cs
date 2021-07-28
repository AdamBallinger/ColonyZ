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
        public Vector2 Position { get; private set; }

        public float X => Position.x;
        public float Y => Position.y;

        /// <summary>
        ///     Entity's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The current tile the entity is on.
        /// </summary>
        public Tile CurrentTile => World.Instance.GetTileAt(Position);

        protected Entity(Tile _tile)
        {
            if (_tile == null) return;
            Position = _tile.Position;
        }

        public abstract Sprite GetSelectionIcon();

        public string GetSelectionName()
        {
            return Name;
        }

        public virtual string GetSelectionDescription()
        {
            return $"Position: ({X:0.#}, {Y:0.#})\n";
        }

        public virtual void SetPosition(Vector2 _pos)
        {
            var tileAtPos = World.Instance.GetTileAt(_pos);
            if (tileAtPos == null)
            {
                return;
            }

            if (tileAtPos != CurrentTile)
            {
                OnTileChanged(tileAtPos);
            }

            Position = _pos;
        }

        /// <summary>
        ///     Event called when the tile the entity is on changes.
        /// </summary>
        /// <param name="_tile"></param>
        protected virtual void OnTileChanged(Tile _tile)
        {
        }

        public Vector2 GetPosition()
        {
            return CurrentTile.Position;
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
            Position = World.Instance.GetTileAt(tileIndex).Position;
        }
    }
}
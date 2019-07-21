using System;
using System.Collections.Generic;
using Models.Map.Tiles.Objects;
using Models.Sprites;
using UnityEngine;

namespace Models.Map.Tiles
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }

        public Vector2 Position => new Vector2(X, Y);

        /// <summary>
        /// The definition of this tile.
        /// </summary>
        public TileDefinition TileDefinition
        {
            get => definition;
            set
            {
                oldDefinition = definition;
                definition = value;

                if (oldDefinition != definition)
                    onTileDefinitionChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Contains a list of tiles that surround this tile.
        /// </summary>
        public List<Tile> Neighbours { get; }

        /// <summary>
        /// Installed tile object for this tile.
        /// </summary>
        public TileObject Object { get; private set; }

        private TileDefinition definition, oldDefinition;
        
        private Action<Tile> onTileChanged;
        
        private Action<Tile> onTileDefinitionChanged;

        /// <summary>
        /// Create a tile at the given x and y from a provided tile definition.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_definition"></param>
        public Tile(int _x, int _y, TileDefinition _definition)
        {
            X = _x;
            Y = _y;
            TileDefinition = _definition;
            Neighbours = new List<Tile>();
        }

        public void SetObject(TileObject _object)
        {
            if (Object != null)
            {
                return;
            }

            for (var xOffset = 0; xOffset < _object.Width; xOffset++)
            {
                for (var yOffset = 0; yOffset < _object.Height; yOffset++)
                {
                    var t = World.Instance.GetTileAt(X + xOffset, Y + yOffset);

                    t.Object = _object;
                    t.Object.OriginTile = this;
                    t.Object.Tile = t;
                    t.onTileChanged?.Invoke(t);
                }
            }
            
            World.Instance.Objects.Add(_object);

            onTileChanged?.Invoke(this);
        }

        public void RemoveObject()
        {
            if (Object == null)
            {
                return;
            }

            World.Instance.Objects.Remove(Object);

            Object = null;

            onTileChanged?.Invoke(this);
        }

        public TileEnterability GetEnterability()
        {
            return Object?.Enterability ?? TileEnterability.Immediate;
        }

        public Tile GetNeighbour(Cardinals _direction)
        {
            switch (_direction)
            {
                case Cardinals.North:
                    return World.Instance.GetTileAt(X, Y + 1);
                case Cardinals.North_East:
                    return World.Instance.GetTileAt(X + 1, Y + 1);
                case Cardinals.East:
                    return World.Instance.GetTileAt(X + 1, Y);
                case Cardinals.South_East:
                    return World.Instance.GetTileAt(X + 1, Y - 1);
                case Cardinals.South:
                    return World.Instance.GetTileAt(X, Y - 1);
                case Cardinals.South_West:
                    return World.Instance.GetTileAt(X - 1, Y - 1);
                case Cardinals.West:
                    return World.Instance.GetTileAt(X - 1, Y);
                case Cardinals.North_West:
                    return World.Instance.GetTileAt(X - 1, Y + 1);
            }

            return null;
        }

        /// <summary>
        /// Registers a callback function for when a tile is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTileChangedCallback(Action<Tile> _callback)
        {
            onTileChanged += _callback;
        }

        /// <summary>
        /// Registers a callback function for when a tiles type is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTileTypeChangedCallback(Action<Tile> _callback)
        {
            onTileDefinitionChanged += _callback;
        }
    }
}
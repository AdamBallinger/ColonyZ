using System;
using System.Collections.Generic;
using Models.Entities;
using Models.Map.Structures;
using Models.Sprites;
using UnityEngine;

namespace Models.Map
{
    public enum Enterability
    {
        Immediate,
        Delayed,
        None
    }

    public class Tile
    {
        public int X { get; }
        public int Y { get; }

        public float MovementModifier { get; }

        /// <summary>
        /// The current tile type of this tile.
        /// </summary>
        public TileType Type
        {
            get { return type; }
            private set
            {
                oldType = Type;
                type = value;

                if (oldType != type)
                    onTileTypeChanged?.Invoke(this);
            }
        }

        public string TileName { get; set; }

        public List<Tile> Neighbours { get; }

        /// <summary>
        /// Installed tile structure for this tile.
        /// </summary>
        public TileStructure Structure { get; private set; }

        public Entity TileEntity { get; private set; }

        private TileType type;
        private TileType oldType;

        private Action<Tile> onTileChanged;
        private Action<Tile> onTileTypeChanged;

        /// <summary>
        /// Create a tile at the given x and y with an optional param for movement speed modifer to change the rate in which
        /// entitys can move across this tile. 1 = normal, 0.5 = half, 2 = double etc.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_tileName"></param>
        /// <param name="_type"></param>
        /// <param name="_movementModifier"></param>
        public Tile(int _x, int _y, string _tileName, TileType _type, float _movementModifier = 1.0f)
        {
            X = _x;
            Y = _y;
            TileName = _tileName;
            Type = _type;
            Neighbours = new List<Tile>();
            MovementModifier = _movementModifier;
        }
        
        public void SetTypeAndName(TileType _type, string _name)
        {
            Type = _type;
            TileName = _name;
        }

        public void InstallStructure(TileStructure _structure)
        {
            if(Structure != null)
            {
                return;
            }

            for(var xOffset = 0; xOffset < _structure.Width; xOffset++)
            {
                for(var yOffset = 0; yOffset < _structure.Height; yOffset++)
                {
                    var t = World.Instance.GetTileAt(X + xOffset, Y + yOffset);

                    t.Structure = _structure;
                    t.Structure.OriginTile = this;
                    t.Structure.Tile = t;
                    t.onTileChanged?.Invoke(t);
                }
            }

            onTileChanged?.Invoke(this);
        }

        public void UninstallStructure()
        {
            if(Structure == null)
            {
                return;
            }

            Structure = null;

            onTileChanged?.Invoke(this);
        }

        public Enterability GetEnterability()
        {
            // TODO: When doors are a thing, return Enterability.Delayed for doors or similar structures.
            return Structure == null ? Enterability.Immediate : Enterability.None;
        }

        /// <summary>
        /// Returns the tile adjacency cardinal for a given tile against this tile.
        /// </summary>
        /// <param name="_tileNeighbour"></param>
        /// <returns></returns>
        public Cardinals GetAdjacentFlag(Tile _tileNeighbour)
        {
            if(_tileNeighbour == null || _tileNeighbour == this) return Cardinals.None;

            var xDist = _tileNeighbour.X - X;
            var yDist = _tileNeighbour.Y - Y;

            // Tile is too far away to be directly adjacent to this tile.
            if(Mathf.Abs(xDist) > 1 || Mathf.Abs(yDist) > 1) return Cardinals.None;

            if(xDist == 0 && yDist == 1) return Cardinals.North;
            if(xDist == 1 && yDist == 1) return Cardinals.North_East;
            if(xDist == 1 && yDist == 0) return Cardinals.East;
            if(xDist == 1 && yDist == -1) return Cardinals.South_East;
            if(xDist == 0 && yDist == -1) return Cardinals.South;
            if(xDist == -1 && yDist == -1) return Cardinals.South_West;
            if(xDist == -1 && yDist == 0) return Cardinals.West;
            if(xDist == -1 && yDist == 1) return Cardinals.North_West;

            return Cardinals.None;
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
            onTileTypeChanged += _callback;
        }
    }
}

using System;
using Models.Entities;
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

        public float MovementModifier { get; private set; } = 1.0f;

        /// <summary>
        /// The current tile type of this tile.
        /// </summary>
        public TileType Type
        {
            get { return type; }
            set
            {
                oldType = Type;
                type = value;

                if (oldType != type)
                    onTileTypeChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Tile Sprite Data containing information on the sprite for this tile type.
        /// </summary>
        public TileSpriteData SpriteData { get; set; }

        /// <summary>
        /// Installed tile structure for this tile.
        /// </summary>
        public TileStructure InstalledStructure { get; private set; }

        public Entity TileEntity { get; private set; }

        private TileType type;
        private TileType oldType;

        private Action<Tile> onTileChanged;
        private Action<Tile> onTileTypeChanged;

        public Tile(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        public void InstallStructure(TileStructure _structure)
        {
            if(InstalledStructure != null)
            {
                return;
            }

            InstalledStructure = _structure;
            InstalledStructure.OriginTile = this; // TODO: Fix this as this wont work in future for objects that are multi-tile.

            onTileChanged?.Invoke(this);
        }

        public void UninstallStructure()
        {
            if(InstalledStructure == null)
            {
                return;
            }

            InstalledStructure = null;

            onTileChanged?.Invoke(this);
        }

        public Enterability GetEnterability()
        {
            // TODO: When doors are a thing, return Enterability.Delayed for doors or similar structures.
            return InstalledStructure == null ? Enterability.Immediate : Enterability.None;
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

using System;

namespace Models.World
{
    public class Tile
    {

        public int X { get; }
        public int Y { get; }

        public TileType Type { get; set; } = TileType.Grass;

        private Action<Tile> onTileChanged;
        private Action<Tile> onTileTypeChanged;

        public Tile(int _x, int _y)
        {
            X = _x;
            Y = _y;
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

        /// <summary>
        /// Unregisters a callback function for when a tile is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void UnregisterTileChangedCallback(Action<Tile> _callback)
        {
            onTileChanged -= _callback;
        }

        /// <summary>
        /// Unregisters a callback function for when a tiles type is changed.
        /// </summary>
        /// <param name="_callback"></param>
        public void UnregisterTileTypeChangedCallback(Action<Tile> _callback)
        {
            onTileTypeChanged -= _callback;
        }
    }
}

using System;

namespace Models.World
{
    public class Tile
    {

        public int X { get; }
        public int Y { get; }

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

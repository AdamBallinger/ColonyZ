using System;
using System.Collections.Generic;
using UnityEngine;
using Models.Entities;
using Models.Pathing;
using System.Collections;
using Models.Entities.Characters;

namespace Models.Map
{
    public class World : IEnumerable
    {
        public static World Instance { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        /// Returns the number of tiles in the world.
        /// </summary>
        public int Size => Width * Height;

        private Tile[] Tiles { get; set; }

	    public List<CharacterEntity> Characters { get; private set; }

	    private Action<Entity> onEntitySpawnCallback;

        private World() { }

        /// <summary>
        /// Creates a new instance of World.Instance
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void CreateWorld(int _width = 200, int _height = 200)
        {
            Instance = new World
            {
                Width = _width,
                Height = _height
            };

            Instance.Tiles = new Tile[Instance.Width * Instance.Height];
            Instance.Characters = new List<CharacterEntity>();

            Instance.PopulateTileArray();
        }

        public void Update()
        {
            foreach(var character in Characters)
            {
                character.Update();
            }

            PathFinder.Instance?.ProcessNext();
        }

        private void PopulateTileArray()
        {
            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    Tiles[x * Width + y] = new Tile(x, y, "Grass_Tile", TileType.Ground);
                }
            }
        }

        /// <summary>
        /// Sets a tile's properties at given world X and Y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_tileName"></param>
        /// <param name="_type"></param>
        public void SetTileAt(int _x, int _y, string _tileName, TileType _type)
        {
            var tile = GetTileAt(_x, _y);

            if(tile != null)
            {
                tile.TileName = _tileName;
                tile.Type = _type;
            }
        }

        /// <summary>
        /// Returns a tile at the given world coordinates.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Tile GetTileAt(int _x, int _y)
        {
            if((_x < 0 || _x >= Width) || (_y < 0 || _y >= Height))
            {
                return null;
            }

            return Tiles[_x * Width + _y];
        }

        /// <summary>
        /// Returns a tile at the given world coordinates.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Tile GetTileAt(float _x, float _y)
        {
            return GetTileAt(Mathf.FloorToInt(_x), Mathf.FloorToInt(_y));
        }

        /// <summary>
        /// Returns a tile at the rounded world position.
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public Tile GetTileAt(Vector2 _pos)
        {
            return GetTileAt(Mathf.FloorToInt(_pos.x), Mathf.FloorToInt(_pos.y));
        }

        /// <summary>
        /// Returns a random world tile.
        /// </summary>
        /// <returns></returns>
        public Tile GetRandomTile()
        {
            return GetTileAt(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height));
        }

	    /// <summary>
	    /// Returns a random world tile with a specified range. The range will be automatically clamped to
	    /// the bounds of the world.
	    /// </summary>
	    /// <param name="_xRangeMin"></param>
	    /// <param name="_yRangeMin"></param>
	    /// <param name="_xRangeMax"></param>
	    /// <param name="_yRangeMax"></param>
	    /// <returns></returns>
	    public Tile GetRandomTile(int _xRangeMin, int _yRangeMin, int _xRangeMax, int _yRangeMax)
        {
            _xRangeMin = Mathf.Clamp(_xRangeMin, 0, Width);
            _xRangeMax = Mathf.Clamp(_xRangeMax, 0, Width);
            _yRangeMin = Mathf.Clamp(_yRangeMin, 0, Height);
            _yRangeMax = Mathf.Clamp(_yRangeMax, 0, Height);

            return GetTileAt(UnityEngine.Random.Range(_xRangeMin, _xRangeMax), UnityEngine.Random.Range(_yRangeMin, _yRangeMax));
        }

        /// <summary>
        /// Returns a list of neighbour tiles for the given tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public List<Tile> GetTileNeighbours(Tile _tile)
        {
            var neighbours = new List<Tile>
            {
                Instance.GetTileAt(_tile.X - 1, _tile.Y + 1),
                Instance.GetTileAt(_tile.X, _tile.Y + 1),
                Instance.GetTileAt(_tile.X + 1, _tile.Y + 1),
                Instance.GetTileAt(_tile.X - 1, _tile.Y),
                Instance.GetTileAt(_tile.X + 1, _tile.Y),
                Instance.GetTileAt(_tile.X - 1, _tile.Y - 1),
                Instance.GetTileAt(_tile.X, _tile.Y - 1),
                Instance.GetTileAt(_tile.X + 1, _tile.Y - 1)
            };

            neighbours.RemoveAll(tile => tile == null);

            return neighbours;
        }

        public void SpawnTileEntity(Tile _tile)
        {
            // TODO: Spawn a tile entity and create a callback to the world controller.
            var entity = new TileEntity(_tile);

            onEntitySpawnCallback?.Invoke(entity);
        }

        public void SpawnCharacter(Tile _tile)
        {
            var entity = new HumanEntity(_tile);
            Characters.Add(entity);

            onEntitySpawnCallback?.Invoke(entity);
        }

        /// <summary>
        /// Registers a callback function invoked when any kind of new entity is spawned into the world.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterEntitySpawnCallback(Action<Entity> _callback)
        {
            onEntitySpawnCallback += _callback;
        }


        #region IEnumerable Implementation

        public IEnumerator<Tile> GetEnumerator()
        {
            return ((IEnumerable<Tile>) Tiles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        #endregion
    }
}

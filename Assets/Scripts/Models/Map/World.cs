using System;
using System.Collections.Generic;
using UnityEngine;
using Models.Entities;
using System.Collections;
using Models.Entities.Characters;
using Models.Map.Pathing;
using Models.Map.Structures;
using Models.Map.Tiles;

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
        /// <param name="_tileTypeChangedCallback"></param>
        /// <param name="_tileChangedCallback"></param>
        public static void CreateWorld(int _width, int _height, Action<Tile> _tileTypeChangedCallback, 
            Action<Tile> _tileChangedCallback)
        {
            Instance = new World
            {
                Width = _width,
                Height = _height
            };

            Instance.Tiles = new Tile[Instance.Width * Instance.Height];
            Instance.Characters = new List<CharacterEntity>();

            Instance.PopulateTileArray(_tileTypeChangedCallback, _tileChangedCallback);
        }

        public void Update()
        {
            foreach (var character in Characters)
            {
                character.Update();
            }

            PathFinder.Instance?.ProcessNext();
        }

        private void PopulateTileArray(Action<Tile> _tileTypeChangeCallback, Action<Tile> _tileChangeCallback)
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var tile = new Tile(x, y, "Grass", TileType.Grass);
                    tile.RegisterTileTypeChangedCallback(_tileTypeChangeCallback);
                    tile.RegisterTileChangedCallback(_tileChangeCallback);
                    Tiles[x * Width + y] = tile;
                }
            }

            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    SetTileNeighbours(Tiles[x * Width + y]);
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

            tile?.SetTypeAndName(_type, _tileName);
        }

        /// <summary>
        /// Returns a tile at the given world coordinates.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Tile GetTileAt(int _x, int _y)
        {
            if ((_x < 0 || _x >= Width) || (_y < 0 || _y >= Height))
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
        private IEnumerable<Tile> GetTileNeighbours(Tile _tile)
        {
            var neighbours = new List<Tile>(8)
            {
                GetTileAt(_tile.X - 1, _tile.Y + 1),
                GetTileAt(_tile.X, _tile.Y + 1),
                GetTileAt(_tile.X + 1, _tile.Y + 1),
                GetTileAt(_tile.X - 1, _tile.Y),
                GetTileAt(_tile.X + 1, _tile.Y),
                GetTileAt(_tile.X - 1, _tile.Y - 1),
                GetTileAt(_tile.X, _tile.Y - 1),
                GetTileAt(_tile.X + 1, _tile.Y - 1)
            };

            neighbours.RemoveAll(tile => tile == null);

            return neighbours;
        }

        private void SetTileNeighbours(Tile _tile)
        {
            _tile.Neighbours.AddRange(GetTileNeighbours(_tile));
        }

        public void SpawnCharacter(Tile _tile)
        {
            var entity = new HumanEntity(_tile);
            Characters.Add(entity);

            onEntitySpawnCallback?.Invoke(entity);
        }

        /// <summary>
        /// Returns if a given tile structure can be placed on a given tile.
        /// </summary>
        /// <param name="_structure"></param>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public bool IsStructurePositionValid(TileStructure _structure, Tile _tile)
        {
            if (_structure == null)
            {
                return false;
            }

            if (_tile == null || _tile.Structure != null)
            {
                return false;
            }
            
            if (_structure.Width <= 1 && _structure.Height <= 1)
            {
                return _structure.CanPlace(_tile);
            }
            
            for (var xOffset = 0; xOffset < _structure.Width; xOffset++)
            {
                for (var yOffset = 0; yOffset < _structure.Height; yOffset++)
                {
                    var t = GetTileAt(_tile.X + xOffset, _tile.Y + yOffset);

                    if (t != null && _structure.CanPlace(t))
                    {
                        continue;
                    }

                    return false;
                }
            }

            return true;

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
            return ((IEnumerable<Tile>)Tiles).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Tiles.GetEnumerator();
        }

        #endregion
    }
}

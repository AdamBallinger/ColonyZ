using System;
using System.Collections.Generic;
using UnityEngine;
using Models.Entities;

namespace Models.Map
{
	public class World 
	{

		public static World Instance { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Tile[,] Tiles { get; private set; }

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

            Instance.Tiles = new Tile[Instance.Width, Instance.Height];
            Instance.Characters = new List<CharacterEntity>();
        }

        public void Update()
        {
            foreach(var character in Characters)
            {
                character.Update();
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

            return Tiles[_x, _y];
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
            var entity = new TileEntity(_tile.X, _tile.Y);

            onEntitySpawnCallback?.Invoke(entity);
        }

        public void SpawnCharacter(Tile _tile)
        {
            var entity = new CharacterEntity(_tile.X, _tile.Y);
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
	}
}

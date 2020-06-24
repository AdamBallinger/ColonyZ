using System;
using System.Collections;
using System.Collections.Generic;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Items;
using ColonyZ.Models.Map.Areas;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ColonyZ.Models.Map
{
    public class World : IEnumerable
    {
        public static World Instance { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        /// <summary>
        ///     Returns the number of tiles in the world.
        /// </summary>
        public int Size => Width * Height;

        public WorldDataProvider WorldProvider { get; private set; }

        public WorldGridLayout WorldGrid { get; private set; }

        public ActionProcessor WorldActionProcessor { get; private set; }

        public List<LivingEntity> Characters { get; private set; }

        public List<ItemEntity> Items { get; private set; }

        public List<TileObject> Objects { get; private set; }

        private Tile[] Tiles { get; set; }

        public WorldGenerator WorldGenerator { get; private set; }

        /// <summary>
        ///     Event called when a new entity is created.
        /// </summary>
        public event Action<Entity> onEntitySpawn;

        /// <summary>
        ///     Event called when an Entity is removed from the world.
        /// </summary>
        public event Action<Entity> onEntityRemoved;

        private World()
        {
        }

        /// <summary>
        ///     Creates a new instance of World.Instance
        /// </summary>
        /// <param name="_worldDataProvider"></param>
        /// <param name="_tileDefinitionChangeListener"></param>
        /// <param name="_tileChangedListener"></param>
        public static void CreateWorld(WorldDataProvider _worldDataProvider, Action<Tile> _tileDefinitionChangeListener,
            Action<Tile> _tileChangedListener)
        {
            Instance = new World
            {
                WorldProvider = _worldDataProvider,
                Width = _worldDataProvider.WorldWidth,
                Height = _worldDataProvider.WorldHeight,
                Tiles = new Tile[_worldDataProvider.WorldWidth * _worldDataProvider.WorldHeight],
                Characters = new List<LivingEntity>(),
                Items = new List<ItemEntity>(),
                Objects = new List<TileObject>()
            };

            TileManager.LoadDefinitions();
            Instance.WorldGrid = new WorldGridLayout(Instance);
            Instance.WorldGenerator = new WorldGenerator(Random.Range(0, 1000000));
            Instance.WorldActionProcessor = new ActionProcessor();
            Instance.PopulateTileArray(_tileDefinitionChangeListener, _tileChangedListener);
            Instance.WorldGrid.BuildWorldGrid();
            NodeGraph.Create();
            AreaManager.Create();
            ZoneManager.Create();
            JobManager.Create();
            RegionManager.Create();
        }

        public void Update()
        {
            for (var i = Characters.Count - 1; i >= 0; i--) Characters[i].Update();

            for (var i = Objects.Count - 1; i >= 0; i--) Objects[i].Update();

            for (var i = Items.Count - 1; i >= 0; i--) Items[i].Update();

            WorldGrid.RebuildDirty();
            PathFinder.Instance?.ProcessNext();
        }

        private void PopulateTileArray(Action<Tile> _tileDefinitionChangeListener, Action<Tile> _tileChangedListener)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var tile = new Tile(x, y, TileManager.GetTileDefinition("Grass"));
                tile.onTileDefinitionChanged += _tileDefinitionChangeListener;
                tile.onTileChanged += _tileChangedListener;
                tile.onTileChanged += WorldGrid.NotifyChunkUpdate;
                Tiles[x * Width + y] = tile;
            }

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var tile = Tiles[x * Width + y];
                tile.DirectNeighbours.AddRange(GetTileNeighbours(tile, false));
                tile.Neighbours.AddRange(GetTileNeighbours(tile));
            }
        }

        #region GetTile Region

        /// <summary>
        ///     Returns a tile at the given world coordinates.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Tile GetTileAt(int _x, int _y)
        {
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return null;

            return Tiles[_x * Width + _y];
        }

        /// <summary>
        ///     Returns a tile at the rounded world position.
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        public Tile GetTileAt(Vector2 _pos)
        {
            _pos += Vector2.one * 0.5f;
            return GetTileAt(Mathf.FloorToInt(_pos.x), Mathf.FloorToInt(_pos.y));
        }

        public Tile GetTileAt(int _index)
        {
            if (_index < 0 || _index >= Size) return null;
            return Tiles[_index];
        }

        public int GetTileIndex(Tile _tile)
        {
            return _tile.X * Width + _tile.Y;
        }

        /// <summary>
        ///     Returns a random world tile.
        /// </summary>
        /// <returns></returns>
        public Tile GetRandomTile(bool _requireEmpty = false)
        {
            return GetRandomTile(1, 1, Width - 1, Height - 1, _requireEmpty);
        }

        /// <summary>
        ///     Return a random tile within the given radius around the given position.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_radius"></param>
        /// <param name="_requireEmpty"></param>
        /// <returns></returns>
        public Tile GetRandomTileAround(int _x, int _y, int _radius, bool _requireEmpty = false)
        {
            return GetRandomTile(_x - _radius, _y - _radius, _x + _radius, _y + _radius, _requireEmpty);
        }

        public Tile GetRandomTileAround(Tile _tile, int _radius, bool _requiresEmpty = false)
        {
            return GetRandomTileAround(_tile.X, _tile.Y, _radius, _requiresEmpty);
        }

        /// <summary>
        ///     Returns a random world tile with a specified range. The range will be automatically clamped to
        ///     the bounds of the world.
        /// </summary>
        /// <param name="_xRangeMin"></param>
        /// <param name="_yRangeMin"></param>
        /// <param name="_xRangeMax"></param>
        /// <param name="_yRangeMax"></param>
        /// <param name="_requiresEmpty"></param>
        /// <returns></returns>
        private Tile GetRandomTile(int _xRangeMin, int _yRangeMin, int _xRangeMax, int _yRangeMax,
            bool _requiresEmpty = false)
        {
            Tile tile = null;

            while (tile == null || _requiresEmpty && tile.GetEnterability() != TileEnterability.Immediate)
            {
                _xRangeMin = Mathf.Clamp(_xRangeMin, 0, Width);
                _xRangeMax = Mathf.Clamp(_xRangeMax, 0, Width);
                _yRangeMin = Mathf.Clamp(_yRangeMin, 0, Height);
                _yRangeMax = Mathf.Clamp(_yRangeMax, 0, Height);

                tile = GetTileAt(Random.Range(_xRangeMin, _xRangeMax),
                    Random.Range(_yRangeMin, _yRangeMax));
            }

            return tile;
        }

        /// <summary>
        ///     Returns a list of neighbour tiles for the given tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <param name="_includeDiagonal"></param>
        /// <returns></returns>
        private IEnumerable<Tile> GetTileNeighbours(Tile _tile, bool _includeDiagonal = true)
        {
            var neighbours = new List<Tile>
            {
                GetTileAt(_tile.X, _tile.Y + 1),
                GetTileAt(_tile.X - 1, _tile.Y),
                GetTileAt(_tile.X + 1, _tile.Y),
                GetTileAt(_tile.X, _tile.Y - 1)
            };

            if (_includeDiagonal)
                neighbours.AddRange(new[]
                {
                    GetTileAt(_tile.X - 1, _tile.Y + 1),
                    GetTileAt(_tile.X + 1, _tile.Y + 1),
                    GetTileAt(_tile.X - 1, _tile.Y - 1),
                    GetTileAt(_tile.X + 1, _tile.Y - 1)
                });

            neighbours.RemoveAll(tile => tile == null);

            return neighbours;
        }

        #endregion

        /// <summary>
        ///     Spawns a new character entity in the world.
        /// </summary>
        /// <param name="_tile"></param>
        public LivingEntity SpawnCharacter(Tile _tile)
        {
            var entity = new HumanEntity(_tile);
            Characters.Add(entity);
            _tile.LivingEntities.Add(entity);

            onEntitySpawn?.Invoke(entity);
            return entity;
        }

        /// <summary>
        ///     Removes a given character entity from the world if it exists.
        /// </summary>
        /// <param name="_entity"></param>
        public void RemoveCharacter(LivingEntity _entity)
        {
            if (!Characters.Contains(_entity)) return;

            Characters.Remove(_entity);
            onEntityRemoved?.Invoke(_entity);
        }

        /// <summary>
        ///     Spawns a new item entity into the world.
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_quantity"></param>
        /// <param name="_tile"></param>
        public bool SpawnItem(Item _item, int _quantity, Tile _tile)
        {
            if (_tile.Item != null) return false;

            var itemEntity = new ItemEntity(_tile, new ItemStack(_item, _quantity));
            _tile.SetItem(itemEntity);
            Items.Add(itemEntity);

            onEntitySpawn?.Invoke(itemEntity);

            return true;
        }

        public void RemoveItem(ItemEntity _entity)
        {
            if (!Items.Contains(_entity)) return;

            _entity.CurrentTile.RemoveItem();
            Items.Remove(_entity);
            onEntityRemoved?.Invoke(_entity);
        }

        /// <summary>
        ///     Returns if a given object can be placed on a given tile.
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public bool IsObjectPositionValid(TileObject _object, Tile _tile)
        {
            if (_tile == null || _tile.HasObject) return false;

            if (_object.Width <= 1 && _object.Height <= 1) return _object.CanPlace(_tile);

            for (var xOffset = 0; xOffset < _object.Width; xOffset++)
            for (var yOffset = 0; yOffset < _object.Height; yOffset++)
            {
                var t = GetTileAt(_tile.X + xOffset, _tile.Y + yOffset);

                if (t != null && _object.CanPlace(t)) continue;

                return false;
            }

            return true;
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
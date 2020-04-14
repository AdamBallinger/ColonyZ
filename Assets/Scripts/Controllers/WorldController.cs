using System.Collections.Generic;
using Controllers.Loaders;
using Controllers.Render;
using Models.AI.Jobs;
using Models.Entities;
using Models.Entities.Living;
using Models.Map;
using Models.Map.Areas;
using Models.Map.Pathing;
using Models.Map.Rooms;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Models.Sprites;
using Models.TimeSystem;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(WorldRenderer))]
    public class WorldController : MonoBehaviour
    {
        [SerializeField] private int worldWidth = 100;
        [SerializeField] private int worldHeight = 100;

        [SerializeField, Range(0, 100)] private int treeSpawnChance = 25;

        [SerializeField, Range(0, 10)] private int initialCharacterCount = 1;

        [SerializeField] private DataLoader dataLoader;

        [SerializeField] private GameObject livingEntityPrefab;
        [SerializeField] private GameObject itemEntityPrefab;

        private WorldRenderer worldRenderer;

        private Dictionary<Tile, SpriteRenderer> tileObjectRenderers;
        private Dictionary<LivingEntity, GameObject> livingEntityObjects;
        private Dictionary<ItemEntity, GameObject> itemEntityObjects;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;

            tileObjectRenderers = new Dictionary<Tile, SpriteRenderer>();
            livingEntityObjects = new Dictionary<LivingEntity, GameObject>();
            itemEntityObjects = new Dictionary<ItemEntity, GameObject>();

            dataLoader.Load();

            worldRenderer = GetComponent<WorldRenderer>();

            SetupWorld();
        }

        private void SetupWorld()
        {
            RoomManager.Create();
            AreaManager.Create();
            TimeManager.Create(6, 30, 1);
            JobManager.Create();
            NodeGraph.Create(worldWidth, worldHeight);

            World.CreateWorld(worldWidth, worldHeight, OnTileDefinitionChanged, OnTileChanged);
            World.Instance.onEntitySpawn += OnEntitySpawn;
            World.Instance.onEntityRemoved += OnEntityRemoved;

            for (var i = 0; i < initialCharacterCount; i++)
            {
                World.Instance.SpawnCharacter(World.Instance.GetRandomTileAround(worldWidth / 2,
                    worldHeight / 2, 5));
            }

            worldRenderer.GenerateWorldMesh(worldWidth, worldHeight);

            foreach (var tile in World.Instance)
            {
                if (tile.IsMapEdge)
                {
                    tile.SetObject(TileObjectCache.GetObject("Tree"), false);
                    continue;
                }

                if (Random.Range(1, 100) <= treeSpawnChance)
                {
                    tile.SetObject(TileObjectCache.GetObject("Tree"), false);
                }
            }

            foreach (var tile in World.Instance)
            {
                if (tile != null && tile.Room == null && !(tile.HasObject && tile.Object.EnclosesRoom))
                {
                    RoomManager.Instance.CheckForRoom(tile);
                }
            }
        }

        private void Update()
        {
            TimeManager.Instance.Update();
            JobManager.Instance.Update();
            World.Instance.Update();

            foreach (var pair in livingEntityObjects)
            {
                pair.Value.transform.position = new Vector2(pair.Key.X, pair.Key.Y);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                for (var i = 0; i < 10; i++)
                {
                    World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TimeManager.Instance.Toggle();
            }
        }

        /// <summary>
        /// Forces an update to a tiles surrounding tiles sprites.
        /// </summary>
        /// <param name="_tile"></param>
        private void UpdateTileNeighbourSprites(Tile _tile)
        {
            foreach (var tile in _tile.Neighbours)
            {
                if (tile.HasObject)
                {
                    tileObjectRenderers[tile].sprite = SpriteCache.GetSprite(tile.Object);
                }
            }
        }

        /// <summary>
        /// Creates the game object for a tile object, and adds it the renderer map.
        /// </summary>
        /// <param name="_tile"></param>
        private void CreateTileObject(Tile _tile)
        {
            var object_GO = new GameObject("Tile Object");
            var object_SR = object_GO.AddComponent<SpriteRenderer>();

            object_GO.transform.position = new Vector2(_tile.X, _tile.Y);
            object_GO.transform.SetParent(_transform);

            tileObjectRenderers.Add(_tile, object_SR);
        }

        /// <summary>
        /// Event for when a tile has been modified. E.g. a wall removed etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileChanged(Tile _tile)
        {
            if (_tile == null)
            {
                return;
            }

            if (!tileObjectRenderers.ContainsKey(_tile))
            {
                CreateTileObject(_tile);
            }

            var spriteRenderer = tileObjectRenderers[_tile];
            spriteRenderer.sprite = SpriteCache.GetSprite(_tile.Object);

            if (_tile.HasObject)
            {
                spriteRenderer.sortingOrder = _tile.Object.GetSortingOrder();
            }

            UpdateTileNeighbourSprites(_tile);
        }

        /// <summary>
        /// Event for when the definition of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileDefinitionChanged(Tile _tile)
        {
            worldRenderer.GenerateWorldMesh(worldWidth, worldHeight);
        }

        /// <summary>
        /// Event for when a new entity is spawned into the world.
        /// </summary>
        /// <param name="_entity"></param>
        public void OnEntitySpawn(Entity _entity)
        {
            if (_entity is LivingEntity living)
            {
                var entity_GO = Instantiate(livingEntityPrefab, _transform);
                entity_GO.transform.position = new Vector2(living.X, living.Y);
                entity_GO.GetComponent<LivingEntityRenderer>().SetEntity(living);

                livingEntityObjects.Add(living, entity_GO);
            }
            else if (_entity is ItemEntity item)
            {
                var item_GO = Instantiate(itemEntityPrefab, _transform);
                item_GO.transform.position = new Vector2(item.X, item.Y);
                item_GO.GetComponent<SpriteRenderer>().sprite = SpriteCache.GetSprite(item.ItemStack.Item);

                itemEntityObjects.Add(item, item_GO);
            }
        }

        /// <summary>
        /// Event for when an entity gets removed from the world.
        /// </summary>
        /// <param name="_entity"></param>
        public void OnEntityRemoved(Entity _entity)
        {
            if (_entity is LivingEntity living)
            {
                if (!livingEntityObjects.ContainsKey(living)) return;

                Destroy(livingEntityObjects[living]);
                livingEntityObjects.Remove(living);
            }
            else if (_entity is ItemEntity item)
            {
                if (!itemEntityObjects.ContainsKey(item)) return;

                Destroy(itemEntityObjects[item]);
                itemEntityObjects.Remove(item);
            }
        }
    }
}
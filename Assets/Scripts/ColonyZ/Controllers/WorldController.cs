using System.Collections.Generic;
using ColonyZ.Controllers.Loaders;
using ColonyZ.Controllers.Render;
using ColonyZ.Controllers.UI;
using ColonyZ.Controllers.UI.Jobs;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Areas;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Models.Saving;
using ColonyZ.Models.Sprites;
using ColonyZ.Models.TimeSystem;
using UnityEngine;

namespace ColonyZ.Controllers
{
    [RequireComponent(typeof(WorldRenderer))]
    public class WorldController : MonoBehaviour
    {
        private Transform _transform;

        [SerializeField] private DataLoader dataLoader;

        [SerializeField] private bool shouldSaveLoad = true;

        [SerializeField] [Range(0, 10)] private int initialCharacterCount = 1;
        [SerializeField] [Range(0, 100)] private int treeSpawnChance = 25;

        [SerializeField] private GameObject itemEntityPrefab;
        [SerializeField] private GameObject livingEntityPrefab;

        [SerializeField] private WorldSizeTypes.WorldSize worldSize = WorldSizeTypes.MEDIUM;

        private Dictionary<ItemEntity, GameObject> itemEntityObjects;
        private Dictionary<LivingEntity, GameObject> livingEntityObjects;
        private Dictionary<Tile, SpriteRenderer> tileObjectRenderers;

        private WorldRenderer worldRenderer;
        private SaveGameHandler saveGameHandler;
        private WorldDataProvider worldProvider;

        private void Awake()
        {
            _transform = transform;
            tileObjectRenderers = new Dictionary<Tile, SpriteRenderer>();
            livingEntityObjects = new Dictionary<LivingEntity, GameObject>();
            itemEntityObjects = new Dictionary<ItemEntity, GameObject>();

            dataLoader.Load();

            worldRenderer = GetComponent<WorldRenderer>();

            // This is only needed when starting the game from the world scene.
            if (WorldLoadSettings.LOAD_TYPE != WorldLoadType.Load &&
                WorldLoadSettings.WORLD_SIZE.Name == null)
            {
                Debug.Log("Defaulting to WorldController defined size: " + worldSize.Name);
                WorldLoadSettings.WORLD_SIZE = worldSize;
            }

            SetupWorld();
        }

        private void OnDestroy()
        {
            if (shouldSaveLoad)
                saveGameHandler.SaveAll();
            AreaManager.Destroy();
            ZoneManager.Destroy();
            TimeManager.Destroy();
            JobManager.Destroy();
            RegionManager.Destroy();
            NodeGraph.Destroy();
        }

        private void SetupWorld()
        {
            saveGameHandler = new SaveGameHandler();
            worldProvider = new WorldDataProvider(WorldLoadSettings.WORLD_SIZE);

            if (WorldLoadSettings.LOAD_TYPE == WorldLoadType.Load)
            {
                saveGameHandler.LoadWorldData(worldProvider);
            }

            worldSize = worldProvider.Size;

            World.CreateWorld(worldProvider, OnTileDefinitionChanged, OnTileChanged);
            World.Instance.onEntitySpawn += OnEntitySpawn;
            World.Instance.onEntityRemoved += OnEntityRemoved;

            FindObjectOfType<ZoneOverlayManager>().Init();
            FindObjectOfType<JobListController>().Init();

            worldRenderer.GenerateWorldMesh(worldSize.Width, worldSize.Height);

            if (WorldLoadSettings.LOAD_TYPE == WorldLoadType.New)
            {
                for (var i = 0; i < initialCharacterCount; i++)
                    World.Instance.SpawnCharacter(World.Instance.GetRandomTileAround(worldSize.Width / 2,
                        worldSize.Height / 2, 5, true));
            }

            foreach (var tile in World.Instance)
            {
                if (tile.IsMapEdge)
                {
                    tile.SetObject(TileObjectCache.GetObject("Tree"), false);
                    continue;
                }

                if (WorldLoadSettings.LOAD_TYPE == WorldLoadType.New)
                {
                    if (Random.Range(1, 100) <= treeSpawnChance)
                        tile.SetObject(TileObjectCache.GetObject("Tree"), false);
                }
            }

            if (WorldLoadSettings.LOAD_TYPE == WorldLoadType.Load)
                saveGameHandler.LoadAll();

            foreach (var tile in World.Instance)
                if (tile.Area == null && !(tile.HasObject && tile.Object.EnclosesRoom))
                    AreaManager.Instance.CheckForArea(tile);

            RegionManager.Instance.BuildRegions();
        }

        private void Update()
        {
            TimeManager.Instance.Update();
            JobManager.Instance.Update();
            World.Instance.Update();

            foreach (var pair in livingEntityObjects)
                pair.Value.transform.position = new Vector2(pair.Key.X, pair.Key.Y);

            if (Input.GetKeyDown(KeyCode.C)) World.Instance.SpawnCharacter(World.Instance.GetRandomTile(true));

            if (Input.GetKeyDown(KeyCode.X))
                for (var i = 0; i < 10; i++)
                    World.Instance.SpawnCharacter(World.Instance.GetRandomTile(true));

            if (Input.GetKeyDown(KeyCode.Space)) TimeManager.Instance.Toggle();
        }

        /// <summary>
        ///     Forces an update to a tiles surrounding tiles sprites.
        /// </summary>
        /// <param name="_tile"></param>
        private void UpdateTileNeighbourSprites(Tile _tile)
        {
            foreach (var tile in _tile.Neighbours)
                if (tile.HasObject)
                    tileObjectRenderers[tile].sprite = SpriteCache.GetSprite(tile.Object);
        }

        /// <summary>
        ///     Creates the game object for a tile object, and adds it the renderer map.
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
        ///     Event for when a tile has been modified. E.g. a wall removed etc.
        /// </summary>
        /// <param name="_tile"></param>
        private void OnTileChanged(Tile _tile)
        {
            if (_tile == null) return;

            if (!tileObjectRenderers.ContainsKey(_tile)) CreateTileObject(_tile);

            var spriteRenderer = tileObjectRenderers[_tile];
            spriteRenderer.sprite = _tile.HasObject ? SpriteCache.GetSprite(_tile.Object) : null;

            if (_tile.HasObject) spriteRenderer.sortingOrder = _tile.Object.GetSortingOrder();

            UpdateTileNeighbourSprites(_tile);
        }

        /// <summary>
        ///     Event for when the definition of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        private void OnTileDefinitionChanged(Tile _tile)
        {
            worldRenderer.GenerateWorldMesh(worldSize.Width, worldSize.Height);
        }

        /// <summary>
        ///     Event for when a new entity is spawned into the world.
        /// </summary>
        /// <param name="_entity"></param>
        private void OnEntitySpawn(Entity _entity)
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
        ///     Event for when an entity gets removed from the world.
        /// </summary>
        /// <param name="_entity"></param>
        private void OnEntityRemoved(Entity _entity)
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
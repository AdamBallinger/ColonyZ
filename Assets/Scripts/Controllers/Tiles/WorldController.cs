using System.Collections.Generic;
using Controllers.UI.Toolbar;
using Models.Entities;
using Models.Map;
using Models.Map.Generation;
using Models.Map.Structures;
using Models.Pathing;
using Models.Sprites;
using UnityEngine;

namespace Controllers.Tiles
{
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; private set; }

        public int worldWidth = 100;
        public int worldHeight = 100;

        public string tileSortingLayerName = "Tiles";

        private Dictionary<Tile, GameObject> tileGameObjectMap;
        private Dictionary<Tile, SpriteRenderer> tileStructureRenderers;
        private Dictionary<CharacterEntity, GameObject> characterEntityGameObjectMap;

        public TileTypeSpriteController TileTypeSpriteController { get; private set; }
        public TileStructureSpriteController TileStructureSpriteController { get; private set; }

        private Transform _transform;

        private void Start()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            Instance.tileGameObjectMap = new Dictionary<Tile, GameObject>();
            Instance.tileStructureRenderers = new Dictionary<Tile, SpriteRenderer>();
            Instance.characterEntityGameObjectMap = new Dictionary<CharacterEntity, GameObject>();

            TileTypeSpriteController = new TileTypeSpriteController();
            TileStructureSpriteController = new TileStructureSpriteController();

            SpriteDataController.RegisterSpriteDataType<TileSpriteData>();
            SpriteDataController.RegisterSpriteDataType<EntitySpriteData>();

            SpriteDataController.Load<TileSpriteData>("Grass_Tile");
            SpriteDataController.Load<TileSpriteData>("Wood_Wall");
            SpriteDataController.Load<TileSpriteData>("Steel_Wall");

            SpriteDataController.Load<EntitySpriteData>("character_bodies");

            TileStructureRegistry.RegisterTileStructure(new WoodWallStructure("Wood_Wall"));
            TileStructureRegistry.RegisterTileStructure(new SteelWallStructure("Steel_Wall"));

            ToolbarController.Instance.AddSubMenuItem("Construction", "Building", "Wood Wall", 
                SpriteCache.GetSprite("tileset_wood_walls_47"), () =>
                {
                    MouseController.Instance.BuildModeController.StartStructureBuild("Wood_Wall");
                });

            ToolbarController.Instance.AddSubMenuItem("Construction", "Building", "Steel Wall", 
                SpriteCache.GetSprite("tileset_steel_walls_47"), () =>
                {
                    MouseController.Instance.BuildModeController.StartStructureBuild("Steel_Wall");
                });     

            NewWorld();
        }

        private void NewWorld()
        {
            World.CreateWorld(worldWidth, worldHeight);
            World.Instance.RegisterEntitySpawnCallback(OnEntitySpawn);

            NodeGraph.Create(World.Instance.Width, World.Instance.Height);

            GenerateTileGameObjects();

            //var worldGen = new WorldGenerator(World.Instance);
            //worldGen.GenerateWorld();

            World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
        }

        private void Update()
        {
            World.Instance?.Update();

            // TODO: Possibly change this as it could be inefficient for large amounts of characters. For now it will do.
            // TODO: Maybe create a single gameobject->transform dict to remove the large amount of GetComponent calls.
            foreach (var pair in characterEntityGameObjectMap)
            {
                pair.Value.transform.position = new Vector2(pair.Key.X, pair.Key.Y);
            }

            if (Input.GetKeyDown(KeyCode.C))
            { 
                World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                for (var i = 0; i < 100; i++)
                {
                    World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
                }
            }
        }

        /// <summary>
        /// Instantiate each gameobject for each world tile.
        /// </summary>
        private void GenerateTileGameObjects()
        {
            // if the world controller object has children then the gameobjects have already been instantiated.
            if (_transform.childCount > 0)
            {
                Debug.LogWarning("Tried to create world tile gameobjects when they were already instantiated!");
                return;
            }

            foreach (var tile in World.Instance)
            {
                // Create the Tile GameObject.
                var tile_GO = new GameObject("Tile");
                tile_GO.transform.position = new Vector2(tile.X, tile.Y);
                tile_GO.transform.SetParent(_transform);

                tileGameObjectMap.Add(tile, tile_GO);

                var tile_sr = tile_GO.AddComponent<SpriteRenderer>();
                tile_sr.sprite = TileTypeSpriteController.GetSprite(tile);
                tile_sr.sortingLayerName = tileSortingLayerName;
                tile_sr.sortingOrder = -10;

                tile.RegisterTileChangedCallback(OnTileChanged);
                tile.RegisterTileTypeChangedCallback(OnTileTypeChange);

                // Create the Tile Structure GameObject.
                var tileStructure_GO = new GameObject("Tile Structure");
                tileStructure_GO.transform.position = new Vector2(tile.X, tile.Y);
                tileStructure_GO.transform.SetParent(tile_GO.transform);

                var tileStructure_SR = tileStructure_GO.AddComponent<SpriteRenderer>();
                tileStructure_SR.sortingLayerName = tileSortingLayerName;
                tileStructure_SR.sortingOrder = -9;

                tileStructureRenderers.Add(tile, tileStructure_SR);
            }
        }

        /// <summary>
        /// Forces an update to a tiles surrounding tiles sprites.
        /// </summary>
        /// <param name="_tile"></param>
        private void UpdateTileNeighbourSprites(Tile _tile)
        {
            foreach (var tile in World.Instance.GetTileNeighbours(_tile))
            {
                if (tile != null)
                {
                    tileStructureRenderers[tile].sprite = TileStructureSpriteController.GetSprite(tile);
                }
            }
        }

        /// <summary>
        /// Callback for when a tile has been modified. E.g. a wall removed etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileChanged(Tile _tile)
        {
            if (_tile != null)
            {
                tileStructureRenderers[_tile].sprite = TileStructureSpriteController.GetSprite(_tile);
                UpdateTileNeighbourSprites(_tile);
            }
        }

        /// <summary>
        /// Callback for when the type of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileTypeChange(Tile _tile)
        {
            tileGameObjectMap[_tile].GetComponent<SpriteRenderer>().sprite = TileTypeSpriteController.GetSprite(_tile);
        }

        /// <summary>
        /// Callback for when a new entity is spawned into the world.
        /// </summary>
        /// <param name="_entity"></param>
        public void OnEntitySpawn(Entity _entity)
        {
            if (_entity is CharacterEntity)
            {
                var char_GO = Instantiate(Resources.Load<GameObject>("Prefabs/Game/Entity_Character"), _transform);
                char_GO.transform.position = new Vector2(_entity.X, _entity.Y);

                // TODO: Set sprites for character GameObject.

                characterEntityGameObjectMap.Add((CharacterEntity)_entity, char_GO);
            }
            else if (_entity is TileEntity)
            {
                // TODO: Create TileEntity GameObject and add it to a GameObject collection (This is still undecided on how it will be stored).
            }
        }
    }
}

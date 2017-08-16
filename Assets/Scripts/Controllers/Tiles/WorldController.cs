using System.Collections.Generic;
using Models.Entities;
using Models.Map;
using Models.Map.Generation;
using Models.Pathing;
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
        private Dictionary<Tile, GameObject> tileStructureGameObjectMap;
        private Dictionary<CharacterEntity, GameObject> characterEntityGameObjectMap;

        private TileTypeSpriteController tileTypeSpritesController;
        private TileStructureSpriteController tileStructureSpriteController;

        private Transform _transform;

        private void Start()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            Instance.tileGameObjectMap = new Dictionary<Tile, GameObject>();
            Instance.tileStructureGameObjectMap = new Dictionary<Tile, GameObject>();
            Instance.characterEntityGameObjectMap = new Dictionary<CharacterEntity, GameObject>();

            Instance.tileTypeSpritesController = gameObject.AddComponent<TileTypeSpriteController>();
            Instance.tileStructureSpriteController = gameObject.AddComponent<TileStructureSpriteController>();

            SpriteDataController.RegisterSpriteDataType<TileSpriteData>();
            SpriteDataController.RegisterSpriteDataType<EntitySpriteData>();

            SpriteDataController.LoadSpriteData(new TileSpriteData("Grass_Tile", true, "tileset_grass_tiles_0",
                "Sprites/Game/Tiles/tileset_grass_tiles"));

            SpriteDataController.LoadSpriteData(new TileSpriteData("Wood_Wall", true, "tileset_wood_walls_",
                "Sprites/Game/Tiles/tileset_wood_walls"));

            NewWorld();
        }

        private void NewWorld()
        {
            World.CreateWorld(worldWidth, worldHeight);
            World.Instance.RegisterEntitySpawnCallback(OnEntitySpawn);

            NodeGraph.Create(World.Instance.Width, World.Instance.Height);

            GenerateTileGameObjects();

            var worldGen = new WorldGenerator(World.Instance);
            worldGen.GenerateWorld();
        }

        private void Update()
        {
            World.Instance?.Update();

            // TODO: Possibly change this as it could be inefficient for large amounts of characters. For now it will do.
            foreach (var pair in characterEntityGameObjectMap)
            {
                pair.Value.transform.position = new Vector2(pair.Key.X, pair.Key.Y);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                World.Instance.SpawnCharacter(World.Instance.GetTileAt(3, 3));
            }
        }

        /// <summary>
        /// Instantiate each gameobject for each world tile.
        /// </summary>
        private void GenerateTileGameObjects()
        {
            // if the world controller object has children then the gameobjects have already been instantiated.
            if (transform.childCount > 0)
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
                tile_sr.sprite = tileTypeSpritesController.GetSprite(tile);
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

                tileStructureGameObjectMap.Add(tile, tileStructure_GO);
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
                    tileStructureGameObjectMap[tile].GetComponent<SpriteRenderer>().sprite = tileStructureSpriteController.GetSprite(tile);
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
                tileStructureGameObjectMap[_tile].GetComponent<SpriteRenderer>().sprite = tileStructureSpriteController.GetSprite(_tile);
                UpdateTileNeighbourSprites(_tile);
            }
        }

        /// <summary>
        /// Callback for when the type of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileTypeChange(Tile _tile)
        {
            tileGameObjectMap[_tile].GetComponent<SpriteRenderer>().sprite = tileTypeSpritesController.GetSprite(_tile);
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

using System.Collections.Generic;
using Controllers.Loaders;
using Controllers.Render;
using Models.Entities;
using Models.Entities.Living;
using Models.Jobs;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Rooms;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Models.Sprites;
using Models.TimeSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; private set; }
        
        public List<Sprite> TileTypesSprites { get; private set; }

        [SerializeField]
        private int worldWidth = 100;
        [SerializeField]
        private int worldHeight = 100;

        [SerializeField, Range(0, 100)]
        private int treeSpawnChance = 25;
        
        [SerializeField]
        private SpriteLoader spriteLoader;
        [SerializeField]
        private TileObjectsLoader objectsLoader;
        [SerializeField]
        private ItemLoader itemsLoader;
        
        [SerializeField]
        private GameObject livingEntityPrefab;
        [SerializeField]
        private GameObject itemEntityPrefab;

        [SerializeField]
        private Texture2D tileTypesTexture;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        
        private Dictionary<Tile, SpriteRenderer> tileObjectRenderers;
        private Dictionary<LivingEntity, GameObject> livingEntityObjects;
        private Dictionary<ItemEntity, GameObject> itemEntityObjects;

        private Transform _transform;

        private void Awake()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            Instance.TileTypesSprites = new List<Sprite>();
            Instance.tileObjectRenderers = new Dictionary<Tile, SpriteRenderer>();
            Instance.livingEntityObjects = new Dictionary<LivingEntity, GameObject>();
            Instance.itemEntityObjects = new Dictionary<ItemEntity, GameObject>();

            spriteLoader.Load();
            objectsLoader.Load();
            itemsLoader.Load();

            SliceTileTypesTexture();
            NewWorld();
        }

        /// <summary>
        /// Cuts the tile types texture up into individual sprites.
        /// </summary>
        private void SliceTileTypesTexture()
        {
            TileTypesSprites.Clear();

            for (var y = 0; y < tileTypesTexture.height / 32; y++)
            {
                for (var x = 0; x < tileTypesTexture.width / 32; x++)
                {
                    var sprite = Sprite.Create(tileTypesTexture, new Rect(x, y, 32, 32), new Vector2(x + 32, y + 32));
                    TileTypesSprites.Add(sprite);
                }
            }
        }

        private void NewWorld()
        {
            RoomManager.Create();
            TimeManager.Create(8, 0, 1);
            
            World.CreateWorld(worldWidth, worldHeight, OnTileDefinitionChanged, OnTileChanged);
            World.Instance.onEntitySpawn += OnEntitySpawn;
            World.Instance.onEntityRemoved += OnEntityRemoved;
            
            JobManager.Create();
            NodeGraph.Create(World.Instance.Width, World.Instance.Height);
            
            GenerateWorldMesh();

            foreach (var tile in World.Instance)
            {
                if (Random.Range(1, 100) <= treeSpawnChance || tile.X == 0 || tile.X == worldWidth - 1 
                    || tile.Y == 0 || tile.Y == worldHeight - 1)
                {
                    tile.SetObject(TileObjectCache.GetObject("Tree"));
                }
            }

            World.Instance.SpawnCharacter(World.Instance.GetRandomTile(worldWidth / 2 - 5, worldHeight / 2 - 5,
                                                                       worldWidth / 2 + 5, worldHeight / 2 + 5));
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
        /// Generates the mesh for the world.
        /// </summary>
        private void GenerateWorldMesh()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();
            }
            else
            {
                // if mesh filter isn't null, then the mesh must be already generated, so abort.
                return;
            }

            var mesh = new Mesh
            {
                name = "World Mesh",
                indexFormat = IndexFormat.UInt16
            };

            var verts = new Vector3[4];
            verts[0] = new Vector3(-0.5f, -0.5f);
            verts[1] = new Vector3(-0.5f, worldHeight - 0.5f);
            verts[2] = new Vector3(worldWidth - 0.5f, worldHeight - 0.5f);
            verts[3] = new Vector3(worldWidth - 0.5f, -0.5f);

            var tris = new int[6];
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 3;
            tris[3] = 1;
            tris[4] = 2;
            tris[5] = 3;
            
            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;
            
            meshFilter.mesh = mesh;
            meshRenderer.material.mainTexture = GenerateMapTexture();
            meshRenderer.material.SetInt("_WorldWidth", worldWidth);
            meshRenderer.material.SetInt("_WorldHeight", worldHeight);
        }
        
        private Texture2D GenerateMapTexture()
        {
            var texture = new Texture2D(worldWidth, worldHeight, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };
            
            var colors = new Color[texture.width * texture.height];
            for (var x = 0; x < worldWidth; x++)
            {
                for (var y = 0; y < worldHeight; y++)
                {
                    var index = x * worldWidth + y;
                    var tileIndex = World.Instance.GetTileAt(x, y).TileDefinition.TextureIndex;
                    colors[index] = new Color(tileIndex, tileIndex, tileIndex);
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();

            return texture;
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
            
            if(!tileObjectRenderers.ContainsKey(_tile))
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
            meshRenderer.material.mainTexture = GenerateMapTexture();
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
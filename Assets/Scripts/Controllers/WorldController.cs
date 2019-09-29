using System.Collections.Generic;
using Models.Entities;
using Models.Entities.Living;
using Models.Jobs;
using Models.Map;
using Models.Map.Pathing;
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

        [SerializeField, Range(1, 100)]
        private int treeSpawnChance = 25;

        [SerializeField]
        private GameObject characterPrefab;

        [SerializeField]
        private string tileObjectsSortingName = "Objects";

        private Dictionary<Tile, SpriteRenderer> tileObjectRenderers;
        private Dictionary<LivingEntity, GameObject> livingEntityObjects;

        [SerializeField]
        private Texture2D tileTypesTexture;

        [SerializeField]
        private SpriteLoader spriteLoader;

        [SerializeField]
        private TileObjectsLoader objectsLoader;

        private MeshFilter meshFilter;

        private Transform _transform;

        private void Awake()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            Instance.TileTypesSprites = new List<Sprite>();
            Instance.tileObjectRenderers = new Dictionary<Tile, SpriteRenderer>();
            Instance.livingEntityObjects = new Dictionary<LivingEntity, GameObject>();

            spriteLoader.Load();
            objectsLoader.Load();

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
            World.CreateWorld(worldWidth, worldHeight, OnTileDefinitionChanged, OnTileChanged);
            World.Instance.onEntitySpawn += OnEntitySpawn;
            
            TimeManager.Create(8, 0, 1);
            JobManager.Create();
            NodeGraph.Create(World.Instance.Width, World.Instance.Height);
            
            GenerateWorldMesh();

            foreach (var tile in World.Instance)
            {
                if (Random.Range(0, treeSpawnChance) == 0 || tile.X == 0 || tile.X == worldWidth - 1 
                    || tile.Y == 0 || tile.Y == worldHeight - 1)
                {
                    tile.SetObject(TileObjectCache.GetObject("Tree"));
                }
            }
            
            World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
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
                for (var i = 0; i < 100; i++)
                {
                    World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
                }
            }
        }

        /// <summary>
        /// Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateWorldMesh()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
            else
            {
                // if mesh filter isn't null, then the mesh must be already generated, so abort.
                return;
            }

            var mesh = new Mesh
            {
                name = "World Mesh",
                indexFormat = IndexFormat.UInt32
            };
            
            var combiner = new CombineInstance[World.Instance.Size];

            for (var x = 0; x < worldWidth; x++)
            {
                for (var y = 0; y < worldHeight; y++)
                {
                    var tile = World.Instance.GetTileAt(x, y);

                    var tileMesh = new Mesh();
                    
                    var tileVerts = new Vector3[4];
                    tileVerts[0] = new Vector2(x - 0.5f, y - 0.5f);
                    tileVerts[1] = new Vector2(x - 0.5f, y + 0.5f);
                    tileVerts[2] = new Vector2(x + 0.5f, y + 0.5f);
                    tileVerts[3] = new Vector2(x + 0.5f, y - 0.5f);
                    
                    var tileUV = new Vector2[4];

                    // Calculate the number of tiles along the X and Y of the texture
                    var textureTileWidth = tileTypesTexture.width / 32;
                    var textureTileHeight = tileTypesTexture.height / 32;

                    var uSize = 1.0f / textureTileWidth; 
                    var vSize = 1.0f / textureTileHeight;

                    // Get the index of the tile in the texture
                    var tileIndex = tile.TileDefinition.TextureIndex;

                    // Calculate tile X and Y inside the texture
                    var tileX = tileIndex % textureTileWidth;
                    var tileY = (tileIndex - tileX) / textureTileWidth;

                    // Generate UV for the bottom left vertex of the tile
                    var u = uSize * tileX;
                    // invert the v as it starts at lower left rather than top left. Minus 1 so v points to bottom vertex
                    var v = vSize * (textureTileHeight - tileY - 1);
                    
                    // Set the UV for the tile quad
                    tileUV[0] = new Vector2(u, v);
                    tileUV[1] = new Vector2(u, v + vSize);
                    tileUV[2] = new Vector2(u + uSize, v + vSize);
                    tileUV[3] = new Vector2(u + uSize, v);

                    var tileTris = new int[6];
                    tileTris[0] = 0;
                    tileTris[1] = 1;
                    tileTris[2] = 3;
                    tileTris[3] = 1;
                    tileTris[4] = 2;
                    tileTris[5] = 3;

                    tileMesh.vertices = tileVerts;
                    tileMesh.triangles = tileTris;
                    tileMesh.uv = tileUV;

                    combiner[x * worldWidth + y].mesh = tileMesh;
                }
            }

            mesh.CombineMeshes(combiner, true, false);
            meshFilter.mesh = mesh;
        }

        /// <summary>
        /// Forces an update to a tiles surrounding tiles sprites.
        /// </summary>
        /// <param name="_tile"></param>
        private void UpdateTileNeighbourSprites(Tile _tile)
        {
            foreach (var tile in _tile.Neighbours)
            {
                if (tile?.Object != null)
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
            
            object_SR.sortingLayerName = tileObjectsSortingName;
            
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
            // TODO: Update world mesh
        }

        /// <summary>
        /// Event for when a new entity is spawned into the world.
        /// </summary>
        /// <param name="_entity"></param>
        public void OnEntitySpawn(Entity _entity)
        {
            if (_entity is HumanEntity)
            {
                var entity_GO = Instantiate(characterPrefab, _transform);
                entity_GO.transform.position = new Vector2(_entity.X, _entity.Y);

                // TODO: Set sprites for character GameObject.

                livingEntityObjects.Add((HumanEntity) _entity, entity_GO);
            }
        }
    }
}
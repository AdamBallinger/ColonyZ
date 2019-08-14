using System.Collections.Generic;
using Models.Entities;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Tiles;
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

        [SerializeField]
        private int worldWidth = 100;
        [SerializeField]
        private int worldHeight = 100;

        [SerializeField]
        private GameObject characterPrefab;

        [SerializeField]
        private string tileSortingLayerName = "Tiles";

        private Dictionary<Tile, SpriteRenderer> tileObjectRenderers;
        private Dictionary<CharacterEntity, GameObject> characterGameObjects;

        private MeshFilter meshFilter;

        [SerializeField]
        private Texture2D tileTypesTexture;

        private Transform _transform;

        private void Awake()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            Instance.tileObjectRenderers = new Dictionary<Tile, SpriteRenderer>();
            Instance.characterGameObjects = new Dictionary<CharacterEntity, GameObject>();

            NewWorld();
        }

        private void NewWorld()
        {
            World.CreateWorld(worldWidth, worldHeight, OnTileDefinitionChanged, OnTileChanged);
            World.Instance.RegisterEntitySpawnCallback(OnEntitySpawn);
            
            TimeManager.Create(8, 0, 1);

            NodeGraph.Create(World.Instance.Width, World.Instance.Height);
            
            GenerateWorldMesh();

            World.Instance.SpawnCharacter(World.Instance.GetRandomTile());
        }

        private void Update()
        {
            TimeManager.Instance.Update();
            World.Instance.Update();
            
            foreach (var pair in characterGameObjects)
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
            
            object_SR.sortingLayerName = tileSortingLayerName;
            
            tileObjectRenderers.Add(_tile, object_SR);
        }

        /// <summary>
        /// Callback for when a tile has been modified. E.g. a wall removed etc.
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

            tileObjectRenderers[_tile].sprite = SpriteCache.GetSprite(_tile.Object);
            UpdateTileNeighbourSprites(_tile);
        }

        /// <summary>
        /// Callback for when the definition of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileDefinitionChanged(Tile _tile)
        {
            // TODO: Update world mesh
        }

        /// <summary>
        /// Callback for when a new entity is spawned into the world.
        /// </summary>
        /// <param name="_entity"></param>
        public void OnEntitySpawn(Entity _entity)
        {
            if (_entity is CharacterEntity)
            {
                var char_GO = Instantiate(characterPrefab, _transform);
                char_GO.transform.position = new Vector2(_entity.X, _entity.Y);

                // TODO: Set sprites for character GameObject.

                characterGameObjects.Add((CharacterEntity) _entity, char_GO);
            }
        }
    }
}
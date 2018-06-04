using System.Collections.Generic;
using Models.Entities;
using Models.Map;
using Models.Map.Structures;
using Models.Pathing;
using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers.Tiles
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WorldController : MonoBehaviour
    {
        public static WorldController Instance { get; private set; }

        public int worldWidth = 100;
        public int worldHeight = 100;

        public string tileSortingLayerName = "Tiles";

        //private Dictionary<Tile, SpriteRenderer> tileTypeRenderer;
        private Dictionary<Tile, SpriteRenderer> tileStructureRenderers;
        private Dictionary<CharacterEntity, GameObject> characterEntityGameObjectMap;

        private MeshFilter meshFilter;

        [SerializeField]
        private Texture2D tileTypesTexture;

        private Transform _transform;

        private void Awake()
        {
            Instance = this;
            Instance._transform = Instance.transform;

            //Instance.tileTypeRenderer = new Dictionary<Tile, SpriteRenderer>();
            Instance.tileStructureRenderers = new Dictionary<Tile, SpriteRenderer>();
            Instance.characterEntityGameObjectMap = new Dictionary<CharacterEntity, GameObject>();

            SpriteDataController.LoadSpriteData();

            TileStructureRegistry.RegisterTileStructure(new WallStructure("Wood_Wall"));
            TileStructureRegistry.RegisterTileStructure(new WallStructure("Steel_Wall"));
            TileStructureRegistry.RegisterTileStructure(new DoorStructure("Wood_Door"));
            TileStructureRegistry.RegisterTileStructure(new DoorStructure("Steel_Door"));

            NewWorld();
        }

        private void NewWorld()
        {
            World.CreateWorld(worldWidth, worldHeight);
            World.Instance.RegisterEntitySpawnCallback(OnEntitySpawn);

            NodeGraph.Create(World.Instance.Width, World.Instance.Height);

            GenerateWorldMesh();

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
                World.Instance?.SpawnCharacter(World.Instance.GetRandomTile());
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                for (var i = 0; i < 100; i++)
                {
                    World.Instance?.SpawnCharacter(World.Instance.GetRandomTile());
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
                    
                    tile.RegisterTileChangedCallback(OnTileChanged);
                    tile.RegisterTileTypeChangedCallback(OnTileTypeChange);

                    var tileMesh = new Mesh();
                    
                    var tileVerts = new Vector3[4];
                    tileVerts[0] = new Vector2(x - 0.5f, y - 0.5f);
                    tileVerts[1] = new Vector2(x - 0.5f, y + 0.5f);
                    tileVerts[2] = new Vector2(x + 0.5f, y + 0.5f);
                    tileVerts[3] = new Vector2(x + 0.5f, y - 0.5f);
                    
                    var tileUV = new Vector2[4];

                    var textureTileWidth = tileTypesTexture.width / 32.0f;
                    var textureTileHeight = tileTypesTexture.height / 32.0f;

                    var uSize = 1.0f / textureTileWidth;
                    var vSize = 1.0f / textureTileHeight;

                    // Tile in sprite sheet (not 0 indexed)
                    var tileSprite = 1;
                    // Get the index of the tile in the texture
                    var tileIndex = tileSprite - 1;

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
                if (tile?.Structure != null)
                {
                    tileStructureRenderers[tile].sprite = SpriteCache.GetSprite(tile.Structure);
                }
            }
        }
        
        /// <summary>
        /// Creates the game object for a tile structure, and adds it the renderer map.
        /// </summary>
        /// <param name="_tile"></param>
        private void CreateStructureGameObject(Tile _tile)
        {
            var structure_GO = new GameObject("Structure");
            var structure_SR = structure_GO.AddComponent<SpriteRenderer>();
            
            structure_GO.transform.position = new Vector2(_tile.X, _tile.Y);
            structure_GO.transform.SetParent(_transform);
            
            structure_SR.sortingLayerName = tileSortingLayerName;
            
            tileStructureRenderers.Add(_tile, structure_SR);
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
            
            if(!tileStructureRenderers.ContainsKey(_tile))
            {
                CreateStructureGameObject(_tile);
            }

            tileStructureRenderers[_tile].sprite = SpriteCache.GetSprite(_tile.Structure);
            UpdateTileNeighbourSprites(_tile);
        }

        /// <summary>
        /// Callback for when the type of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileTypeChange(Tile _tile)
        {
            // TODO: Update just this tiles mesh to the new texture
            //tileTypeRenderer[_tile].sprite = SpriteCache.GetSprite(_tile.TileName, 0);
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

                characterEntityGameObjectMap.Add((CharacterEntity) _entity, char_GO);
            }
            else if (_entity is TileEntity)
            {
                // TODO: Create TileEntity GameObject and add it to a GameObject collection (This is still undecided on how it will be stored).
            }
        }
    }
}
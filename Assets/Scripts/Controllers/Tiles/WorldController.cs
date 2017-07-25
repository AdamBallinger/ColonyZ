using System.Collections.Generic;
using Models.World;
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

	    private TileTypeSpriteController tileTypeSpritesController;
	    private TileStructureSpriteController tileStructureSpriteController;

	    private Transform _transform;

		private void Start()
		{
		    Instance = this;
		    Instance._transform = Instance.transform;

            Instance.tileGameObjectMap = new Dictionary<Tile, GameObject>();
            Instance.tileStructureGameObjectMap = new Dictionary<Tile, GameObject>();

            Instance.tileTypeSpritesController = gameObject.AddComponent<TileTypeSpriteController>();
		    Instance.tileStructureSpriteController = gameObject.AddComponent<TileStructureSpriteController>();

		    NewWorld();
		}

        private void NewWorld()
        {
            World.CreateWorld();

            for(var x = 0; x < World.Instance.Width; x++)
            {
                for(var y = 0; y < World.Instance.Height; y++)
                {
                    World.Instance.Tiles[x, y] = new Tile(x, y)
                    {
                        SpriteData = new TileSpriteData
                        {
                            IsTileSet = true,
                            SpriteName = "tileset_grass_",
                            SpriteResourceLocation = "Sprites/Game/Tiles/tileset_grass_tiles"
                        }
                    };
                }
            }

            GenerateTileGameObjects();
        }

        /// <summary>
        /// Instantiate each gameobject for each world tile.
        /// </summary>
        private void GenerateTileGameObjects()
        {
            // if the world controller object has children then the gameobjects have already been instantiated.
            if(transform.childCount > 0)
            {
                Debug.LogWarning("Tried to create world tile gameobjects when they were already instantiated!");
                return;
            }

            foreach (var tile in World.Instance.Tiles)
            {
                // Create the Tile GameObject.
                var tile_GO = new GameObject($"Tile: X:{tile.X} Y: {tile.Y}");
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
                tileStructure_SR.sortingOrder = -9;

                tileStructureGameObjectMap.Add(tile, tileStructure_GO);
            }
        }

        /// <summary>
        /// Callback for when a tile has been modified. E.g. a wall removed etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileChanged(Tile _tile)
        {
            if(_tile != null)
            {
                tileStructureGameObjectMap[_tile].GetComponent<SpriteRenderer>().sprite = tileStructureSpriteController.GetSprite(_tile);
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
	}
}

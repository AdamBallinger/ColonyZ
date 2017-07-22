using System.Collections.Generic;
using Models.World;
using UnityEngine;

namespace Controllers.Tiles
{
	public class WorldController : MonoBehaviour 
	{
        public static WorldController Instance { get; private set; }

	    private Dictionary<Tile, GameObject> tileGameObjectMap;

		private void Start()
		{
		    Instance = this;
		    NewWorld();
		}

        private void NewWorld()
        {
            World.CreateWorld();

            tileGameObjectMap = new Dictionary<Tile, GameObject>();

            for(var x = 0; x < World.Instance.Width; x++)
            {
                for(var y = 0; y < World.Instance.Height; y++)
                {
                    World.Instance.Tiles[x, y] = new Tile(x, y);
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
                var tile_GO = new GameObject($"Tile: X:{tile.X} Y: {tile.Y}");
                tile_GO.transform.position = new Vector2(tile.X, tile.Y);
                tile_GO.transform.SetParent(transform);

                tileGameObjectMap.Add(tile, tile_GO);

                //tile_GO.AddComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Game/Tiles/tileset_grass_tiles")[2];

                tile.RegisterTileChangedCallback(OnTileChanged);
                tile.RegisterTileTypeChangedCallback(OnTileTypeChange);
            }
        }

        /// <summary>
        /// Callback for when a tile has been modified. E.g. a wall removed etc.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileChanged(Tile _tile)
        {
            
        }

        /// <summary>
        /// Callback for when the type of a tile has been changed.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileTypeChange(Tile _tile)
        {
            
        }
	}
}

using System.Collections.Generic;
using Controllers;
using Models.Pathing;
using UnityEngine;

namespace Models.Map.Generation
{
	public class WorldGenerator
	{

	    private World world;

	    private Dictionary<Tile, int> centerTiles;

        public WorldGenerator(World _world)
        {
            world = _world;
            centerTiles = new Dictionary<Tile, int>();
        }

        public void GenerateWorld()
        {
            GenerateCenterTiles();
            ExpandCenterTiles();
            NodeGraph.Instance?.UpdateGraph(0, 0, world.Width, world.Height, 0);
        }

        private void GenerateCenterTiles()
        {
            centerTiles.Clear();

            var centerTileCount = Random.Range(100, 150);

            for(var i = 0; i < centerTileCount; i++)
            {
                var ranNum = Random.Range(0, 9);
                var tile = world.GetRandomTile();

                if(centerTiles.ContainsKey(tile))
                {
                    i--;
                    continue;
                }

                centerTiles.Add(tile, ranNum);
            }
        }

	    //private TileSpriteData woodWallData = new TileSpriteData
	    //{
	    //    IsTileSet = true,
	    //    SpriteName = "tileset_wood_walls_",
	    //    ResourceLocation = "Sprites/Game/Tiles/tileset_wood_walls"
	    //};

        private void ExpandCenterTiles()
        {
            foreach(var tile in world.Tiles)
            {
                var closest = GetClosestCenterTileToTile(tile);

                if(centerTiles[closest] == 1)
                    tile.InstallStructure(new TileStructure(1, 1, "Wood_Wall", TileStructureType.Multi_Tile, 
                        SpriteDataController.GetSpriteDataFor<TileSpriteData>("Wood_Wall")));
            }
        }

        private Tile GetClosestCenterTileToTile(Tile _tile)
        {
            Tile closestTile = null;
            var closestDist = Mathf.Infinity;

            foreach(var centerTile in centerTiles)
            {
                var currentDistance = GetDistanceBetweenTiles(centerTile.Key, _tile);

                if(currentDistance < closestDist)
                {
                    closestTile = centerTile.Key;
                    closestDist = currentDistance;
                }
            }

            return closestTile;
        }

        private float GetDistanceBetweenTiles(Tile _tileA, Tile _tileB)
        {
            return Vector2.Distance(new Vector2(_tileA.X, _tileA.Y), new Vector2(_tileB.X, _tileB.Y));
        }
	}
}

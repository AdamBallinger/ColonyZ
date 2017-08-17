using System.Collections.Generic;
using System.Linq;
using Controllers;
using Models.Pathing;
using UnityEngine;

namespace Models.Map.Generation
{
    public class WorldGenerator
    {

        private World world;

        private Dictionary<Tile, int> centerTiles;
        private List<Tile> filledTiles;

        public WorldGenerator(World _world)
        {
            world = _world;
            centerTiles = new Dictionary<Tile, int>();
            filledTiles = new List<Tile>();
        }

        public void GenerateWorld()
        {
            GenerateCenterTiles();
            ExpandCenterTiles();
            Smooth();

            NodeGraph.Instance?.UpdateGraph(0, 0, world.Width, world.Height, 0);
        }

        private void GenerateCenterTiles()
        {
            centerTiles.Clear();

            var centerTileCount = Random.Range(100, 150);

            for (var i = 0; i < centerTileCount; i++)
            {
                var ranNum = Random.Range(0, 9);
                var tile = world.GetRandomTile();

                if (centerTiles.ContainsKey(tile))
                {
                    i--;
                    continue;
                }

                centerTiles.Add(tile, ranNum);
            }
        }

        private void ExpandCenterTiles()
        {
            foreach (var tile in world)
            {
                var closest = GetClosestCenterTileToTile(tile);

                if (centerTiles[closest] == 1)
                {
                    tile.InstallStructure(new TileStructure(1, 1, "Steel_Wall", TileStructureType.Multi_Tile));
                    filledTiles.Add(tile);
                }
            }
        }

        private void Smooth()
        {
            foreach (var tile in filledTiles)
            {
                var tileNeighbourFilled = world.GetTileNeighbours(tile).Count(neighbour => filledTiles.Contains(neighbour));

                switch (tileNeighbourFilled)
                {
                    case 0:
                    case 1:
                    case 3:
                    case 2:
                        tile.UninstallStructure();
                        break;
                }
            }
        }

        private Tile GetClosestCenterTileToTile(Tile _tile)
        {
            Tile closestTile = null;
            var closestDist = Mathf.Infinity;

            foreach (var centerTile in centerTiles)
            {
                var currentDistance = GetDistanceBetweenTiles(centerTile.Key, _tile);

                if (currentDistance < closestDist)
                {
                    closestTile = centerTile.Key;
                    closestDist = currentDistance;
                }
            }

            return closestTile;
        }

        private float GetDistanceBetweenTiles(Tile _tileA, Tile _tileB)
        {
            var dx = Mathf.Abs(_tileA.X - _tileB.X);
            var dy = Mathf.Abs(_tileA.Y - _tileB.Y);

            return dx + dy;
            //return Vector2.Distance(new Vector2(_tileA.X, _tileA.Y), new Vector2(_tileB.X, _tileB.Y));
        }
    }
}

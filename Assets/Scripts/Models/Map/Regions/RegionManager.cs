using System;
using System.Collections.Generic;
using Models.Map.Tiles;
using Utils;

namespace Models.Map.Regions
{
    public class RegionManager
    {
        public static RegionManager Instance { get; private set; }

        private List<Region> Regions { get; set; }

        private List<Region> newRegions = new List<Region>();

        /// <summary>
        /// Event called when regions get updated.
        /// </summary>
        public event Action regionsUpdateEvent;

        private RegionManager()
        {
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new RegionManager();
        }

        /// <summary>
        /// Update regions when a tile has an object placed / removed.
        /// </summary>
        /// <param name="_tile"></param>
        public void Update(Tile _tile)
        {
            var rootChunk = World.Instance.WorldGrid.GetChunkAt(_tile.X, _tile.Y);

            var chunksToUpdate = new List<WorldChunk>();
            chunksToUpdate.Add(rootChunk);

            // TODO: Fix removing objects not telling linked regions to update their edge spans.
            if (_tile.Region != null && _tile.Region.EdgeTiles.Contains(_tile))
            {
                chunksToUpdate.AddRange(World.Instance.WorldGrid.GetChunkNeighbours(rootChunk));
            }

            foreach (var chunk in chunksToUpdate)
            {
                foreach (var tile in chunk.Tiles)
                {
                    if (tile.Region != null)
                    {
                        DeleteRegion(tile.Region);
                    }
                }
            }

            foreach (var chunk in chunksToUpdate)
            {
                FloodChunk(chunk);
            }

            foreach (var region in newRegions)
            {
                region.CalculateBoundaryTiles();
            }

            newRegions.Clear();
            regionsUpdateEvent?.Invoke();
        }

        /// <summary>
        /// Run first time build of regions. This should only be called once when the world is first
        /// instantiated.
        /// </summary>
        public void BuildRegions()
        {
            Regions = new List<Region>();

            foreach (var chunk in World.Instance.WorldGrid.Chunks)
            {
                FloodChunk(chunk);
            }

            foreach (var region in newRegions)
            {
                region.CalculateBoundaryTiles();
            }

            newRegions.Clear();
        }

        private void FloodChunk(WorldChunk _chunk)
        {
            foreach (var tile in _chunk.Tiles)
            {
                if (tile.Region != null) continue;
                if (tile.GetEnterability() == TileEnterability.None) continue;

                // Doors are their own region.
                if (tile.GetEnterability() == TileEnterability.Delayed)
                {
                    CreateRegion(new List<Tile> {tile});
                    continue;
                }

                FloodFiller.Flood(tile,
                    t => t != null
                         && _chunk.Contains(t)
                         && t.GetEnterability() == TileEnterability.Immediate,
                    t => t != null,
                    CreateRegion);
            }
        }

        private void CreateRegion(List<Tile> _tiles)
        {
            if (_tiles == null || _tiles.Count <= 0)
            {
                return;
            }

            var region = new Region();

            foreach (var tile in _tiles)
            {
                region.Add(tile);
            }

            Regions.Add(region);
            newRegions.Add(region);
        }

        private void DeleteRegion(Region _region)
        {
            foreach (var link in _region.Links)
            {
                link.Unassign(_region);
            }

            foreach (var tile in _region.Tiles)
            {
                tile.Region = null;
            }

            Regions.Remove(_region);
        }
    }
}
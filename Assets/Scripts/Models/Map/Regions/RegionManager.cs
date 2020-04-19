﻿using System;
using System.Collections.Generic;
using System.Linq;
using Models.Map.Tiles;
using Utils;

namespace Models.Map.Regions
{
    public class RegionManager
    {
        public static RegionManager Instance { get; private set; }

        private List<Region> Regions { get; set; }

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

            // TODO: Improve how regions are updated.
            // This is too slow. Need to be able to detect which neighbour chunk is affected by the
            // updated tile instead of just updating all neighbour chunks which is too slow.
            // Also doesn't update edge spans for neighbour regions when a object is removed
            // from the edge of the region.

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

                FloodChunk(chunk);
            }

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
                    set => CreateRegion(set.ToList()));
            }
        }

        private void CreateRegion(List<Tile> _tiles)
        {
            if (_tiles == null || _tiles.Count <= 0)
            {
                return;
            }

            var region = new Region();

            _tiles.ForEach(t => region.Add(t));

            Regions.Add(region);
            region.CalculateBoundaryTiles();
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
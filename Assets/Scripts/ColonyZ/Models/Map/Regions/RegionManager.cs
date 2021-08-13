using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Areas;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Utils;

namespace ColonyZ.Models.Map.Regions
{
    public class RegionManager
    {
        public static RegionManager Instance { get; private set; }
        
        public List<Region> Regions { get; private set; }

        /// <summary>
        ///     Event called when regions get updated.
        /// </summary>
        public event Action regionsUpdateEvent;

        private List<Region> newRegions = new List<Region>();
        
        private RegionManager()
        {
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new RegionManager();
        }

        public static void Destroy()
        {
            RegionLinksDatabase.Clear();
            Instance = null;
        }

        /// <summary>
        ///     Update regions when a chunk was modified this frame.
        /// </summary>
        /// <param name="_chunk"></param>
        public void UpdateChunk(WorldChunk _chunk)
        {
            foreach (var tile in _chunk.Tiles)
            {
                if (tile.Region != null)
                {
                    DeleteRegion(tile.Region);
                }
            }

            FloodChunk(_chunk);
        }

        /// <summary>
        ///     Notifies the manager that all new regions have been built.
        /// </summary>
        public void NotifyRegionsUpdated()
        {
            foreach (var region in newRegions)
            {
                region.BuildLinks();
            }
            
            AreaManager.Instance.OnRegionsUpdated(newRegions);
            newRegions.Clear();
            
            regionsUpdateEvent?.Invoke();
        }

        /// <summary>
        ///     Run first time build of regions.
        /// </summary>
        public void BuildRegions()
        {
            if (Regions != null) return;

            Regions = new List<Region>();

            foreach (var chunk in World.Instance.WorldGrid.Chunks) FloodChunk(chunk);

            foreach (var region in newRegions) region.BuildLinks();

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
                         && (t.GetEnterability() == TileEnterability.Immediate ||
                             t.HasObject && t.Object is FurnitureObject),
                    t => t != null,
                    CreateRegion);
            }
        }

        private void CreateRegion(List<Tile> _tiles)
        {
            if (_tiles == null || _tiles.Count <= 0) return;

            var region = new Region();

            foreach (var tile in _tiles) region.Add(tile);

            Regions.Add(region);
            newRegions.Add(region);
        }

        private void DeleteRegion(Region _region)
        {
            foreach (var link in _region.Links) link.Unassign(_region);

            foreach (var tile in _region.Tiles)
            {
                tile.Region = null;
            }
            
            _region.Area?.UnassignRegion(_region);
            _region.Area = null;

            Regions.Remove(_region);
        }
    }
}
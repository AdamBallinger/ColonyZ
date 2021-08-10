using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map.Areas
{
    public class AreaManager
    {
        public static AreaManager Instance { get; private set; }

        public List<Area> Areas { get; }

        /// <summary>
        ///     Event called after new area have been created, or old areas deleted.
        /// </summary>
        public event Action areasUpdatedEvent;

        private AreaManager()
        {
            Areas = new List<Area>();
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new AreaManager();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public int GetAreaID(Area _area)
        {
            return Areas.IndexOf(_area) + 1;
        }

        /// <summary>
        ///     Rebuilds the entire area data structure for the entire world.
        /// </summary>
        public void Rebuild()
        {
            var visited = new List<Region>();
            foreach (var region in RegionManager.Instance.Regions)
            {
                if (visited.Contains(region)) continue;
                
                visited.Add(region);

                // List of regions to iterate over that are connected to the current region.
                var regions = new List<Region>();
                var areaTiles = new List<Tile>();

                if (region.Area == null)
                {
                    if (!region.IsDoor)
                        areaTiles.AddRange(region.Tiles);
                    
                    regions.Add(region);
                    for (var i = 0; i < regions.Count; i++)
                    {
                        var r = regions[i];
                        foreach (var link in r.Links)
                        {
                            var other = link.GetOther(r);
                            if (other == null || visited.Contains(other)) continue;
                            visited.Add(other);

                            if (other.Area == null)
                            {
                                // If region is a door then stop since areas are separated by doors.
                                if (other.IsDoor) continue;
                                
                                areaTiles.AddRange(other.Tiles);
                                regions.Add(other);
                            }
                        }
                    }
                }
                
                CreateArea(areaTiles);
            }
            
            areasUpdatedEvent?.Invoke();
        }

        public void OnRegionsUpdated(List<Region> _newRegions)
        {
            foreach (var region in _newRegions)
            {
                if (region.Area != null || region.IsDoor) continue;

                var visitied = new List<Region>();
                // List of regions connected to this new region that already have an area assigned.
                var regionsWithArea = new List<Region>();
                // List of regions to check.
                var regions = new List<Region>();
                // List of tiles that are part of a new region without an area.
                var areaTiles = new List<Tile>();
                
                visitied.Add(region);
                regions.Add(region);
                areaTiles.AddRange(region.Tiles);

                for (var i = 0; i < regions.Count; i++)
                {
                    var r = regions[i];
                    foreach (var link in r.Links)
                    {
                        var other = link.GetOther(r);
                        if (other == null || visitied.Contains(other)) continue;
                        visitied.Add(other);
                        if (other.IsDoor) continue;
                        regions.Add(other);
                        
                        if (other.Area == null)
                        {
                            // Areas stop at doors.
                            if (other.IsDoor) continue;
                            
                            areaTiles.AddRange(other.Tiles);
                        }
                        else
                        {
                            if (!regionsWithArea.Contains(other) && !other.IsDoor)
                                regionsWithArea.Add(other);
                        }
                    }
                }
                
                foreach (var r in regionsWithArea) areaTiles.AddRange(r.Tiles);
                var newArea = CreateArea(areaTiles);

                if (regionsWithArea.Count > 0)
                {
                    var sumOfRegionsWithArea = regionsWithArea.Sum(r => r.Tiles.Count);

                    if (sumOfRegionsWithArea < regionsWithArea[0].Area.Size)
                    {
                        MergeAreas(newArea, regionsWithArea[0].Area);
                    }
                }
            }
            
            areasUpdatedEvent?.Invoke();
        }

        private Area CreateArea(List<Tile> _tiles)
        {
            if (_tiles != null && _tiles.Count > 0)
            {
                var area = new Area();
                _tiles.ForEach(t => area.AssignTile(t));
                Areas.Add(area);

                return area;
            }

            return null;
        }

        /// <summary>
        ///     Merges target area into the source area.
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_source"></param>
        private void MergeAreas(Area _target, Area _source)
        {
            _target.ReleaseTo(_source);
        }
    }
}
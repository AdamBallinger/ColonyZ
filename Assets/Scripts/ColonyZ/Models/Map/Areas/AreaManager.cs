using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Regions;

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
                
                // List of regions potentially creating a new area.
                var areaRegions = new List<Region>();

                if (region.Area == null)
                {
                    if (!region.IsDoor)
                        areaRegions.Add(region);
                    
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
                                
                                areaRegions.Add(other);
                                regions.Add(other);
                            }
                        }
                    }
                }
                
                CreateArea(areaRegions);
            }
            
            areasUpdatedEvent?.Invoke();
            
            GenerateAreaLinks();
        }

        public void OnRegionsUpdated(List<Region> _newRegions)
        {
            foreach (var region in _newRegions)
            {
                if (region.Area != null || region.IsDoor) continue;

                // List of regions already checked.
                var visitied = new List<Region>();
                // List of regions connected to this new region that already have an area assigned.
                var regionsWithArea = new List<Region>();
                // List of regions to check.
                var regions = new List<Region>();
                // List of regions that make up the potential new area.
                var areaRegions = new List<Region>();
                
                visitied.Add(region);
                regions.Add(region);
                areaRegions.Add(region);

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
                            areaRegions.Add(other);
                        }
                        else
                        {
                            if (!regionsWithArea.Contains(other) && !other.IsDoor)
                                regionsWithArea.Add(other);
                        }
                    }
                }

                areaRegions.AddRange(regionsWithArea);
                var newArea = CreateArea(areaRegions);
                
                if (regionsWithArea.Count > 0)
                {
                    var sumOfRegionsWithArea = regionsWithArea.Sum(r => r.Tiles.Count);
                
                    if (sumOfRegionsWithArea < regionsWithArea[0].Area.SizeInTiles)
                    {
                        MergeAreas(newArea, regionsWithArea[0].Area);
                    }
                }
            }
            
            areasUpdatedEvent?.Invoke();
            
            GenerateAreaLinks();
        }

        private Area CreateArea(List<Region> _regions)
        {
            if (_regions != null && _regions.Count > 0)
            {
                var area = new Area();
                _regions.ForEach(r => area.AssignRegion(r));
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

        private void GenerateAreaLinks()
        {
            foreach (var area in Areas)
            {
                area.LinkedAreas.Clear();
                area.AddConnection(area);
            }

            foreach (var door in RegionManager.Instance.DoorRegions)
            {
                var tile = door.Tiles[0];
                var n = World.Instance.GetTileAt(tile.X, tile.Y + 1);
                var s = World.Instance.GetTileAt(tile.X, tile.Y - 1);

                // If the tiles north and south of door have different area ids then they can be connected.
                if (n != null && s != null && n.Area?.AreaID != s.Area?.AreaID)
                {
                    n.Area?.AddConnection(s.Area);
                    s.Area?.AddConnection(n.Area);
                    continue;
                }

                var e = World.Instance.GetTileAt(tile.X + 1, tile.Y);
                var w = World.Instance.GetTileAt(tile.X - 1, tile.Y);

                if (e != null && w != null && e.Area?.AreaID != w.Area?.AreaID)
                {
                    e.Area?.AddConnection(w.Area);
                    w.Area?.AddConnection(e.Area);
                }
            }

            foreach (var area in Areas)
            {
                var checkedAreas = new List<Area>();
                var areas = LinkArea(area, checkedAreas);
                
                foreach (var a in areas) area.AddConnection(a);
            }
        }

        private List<Area> LinkArea(Area _area, List<Area> _checkedAreas)
        {
            foreach (var area in _area.LinkedAreas)
            {
                if (_checkedAreas.Contains(area)) continue;
                
                _checkedAreas.Add(area);
                LinkArea(area, _checkedAreas);
            }

            return _checkedAreas;
        }
    }
}
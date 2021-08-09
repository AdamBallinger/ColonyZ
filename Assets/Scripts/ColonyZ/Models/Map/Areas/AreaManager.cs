using System;
using System.Collections.Generic;
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
        ///     Rebuilds the entire room data structure for the entire world.
        /// </summary>
        public void Rebuild()
        {
            var visited = new List<Region>();
            foreach (var region in RegionManager.Instance.Regions)
            {
                if (visited.Contains(region)) continue;
                
                visited.Add(region);

                var regions = new List<Region>();
                var areaTiles = new List<Tile>();
                areaTiles.AddRange(region.Tiles);

                if (region.Area == null)
                {
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
                                areaTiles.AddRange(other.Tiles);
                                regions.Add(other);
                            }
                        }
                    }
                }
                
                CreateArea(areaTiles);
            }
        }

        private void RemoveArea(Area _area)
        {
            if (_area == null) return;

            _area.ReleaseTiles();
            Areas.Remove(_area);
            areasUpdatedEvent?.Invoke();
        }

        private void CreateArea(List<Tile> _tiles)
        {
            if (_tiles != null && _tiles.Count > 0)
            {
                var area = new Area();
                _tiles.ForEach(t => area.AssignTile(t));
                Areas.Add(area);
                areasUpdatedEvent?.Invoke();
            }
        }

        /// <summary>
        ///     Merges target area into the source area.
        /// </summary>
        /// <param name="_target"></param>
        /// <param name="_source"></param>
        private void MergeAreas(Area _target, Area _source)
        {
            _target.ReleaseTo(_source);
            RemoveArea(_target);
            areasUpdatedEvent?.Invoke();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Models.Map.Tiles;
using Utils;

namespace Models.Map.Regions
{
    public class RegionManager
    {
        public static RegionManager Instance { get; private set; }

        /// <summary>
        /// Maximum size in both width and height a region can be.
        /// </summary>
        public static readonly int REGION_MAX_SIZE = 16;

        public List<Region> Regions { get; private set; }

        private RegionManager()
        {
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new RegionManager();
        }

        public int GetID(Region _region)
        {
            return Regions.Contains(_region) ? Regions.IndexOf(_region) + 1 : -1;
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
                foreach (var tile in chunk.Tiles)
                {
                    if (tile.GetEnterability() != TileEnterability.Immediate) continue;
                    if (GetID(tile.Region) != -1) continue;

                    FloodFiller.Flood(tile, t =>
                    {
                        return t != null
                               && chunk.Contains(t)
                               && t.GetEnterability() == TileEnterability.Immediate;
                    }, t => { return t != null; }, set => CreateRegion(set.ToList()));
                }
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
        }
    }
}
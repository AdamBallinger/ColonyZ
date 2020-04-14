using System.Collections.Generic;
using Models.Map.Tiles;

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
            Instance.BuildRegions();
        }

        /// <summary>
        /// Run first time build of regions. This should only be called once when the world is first
        /// instantiated.
        /// </summary>
        private void BuildRegions()
        {
            Regions = new List<Region>();

            foreach (var tile in World.Instance)
            {
                if (tile.GetEnterability() != TileEnterability.None)
                {
                    Flood(tile);
                }
            }

            // Flood every tile, limit to max size, and create regions for each flooded shape.
            // X and Y of the tile being flooded should be checked if it is divisible by the max size
            // so split regions still align to a grid layout.
        }

        private void Flood(Tile _source)
        {
        }
    }
}
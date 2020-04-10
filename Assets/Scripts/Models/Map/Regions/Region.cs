using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Regions
{
    /// <summary>
    /// Regions are spacial aware areas of the map. They are used for checking reachability, and finding
    /// nearest objects / items / entities etc.
    /// Regions are defined a max size in RegionManager.cs and will always be built to follow a grid like
    /// layout.
    /// </summary>
    public class Region
    {
        public List<Tile> Tiles { get; private set; }

        public Region()
        {
            Tiles = new List<Tile>();
        }
    }
}
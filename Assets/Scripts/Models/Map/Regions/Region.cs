using System.Collections.Generic;
using Models.Map.Tiles;
using UnityEngine;

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
        public HashSet<Tile> Tiles { get; private set; }

        public void Add(Tile _tile)
        {
            if (Tiles == null) Tiles = new HashSet<Tile>();

            if (Tiles.Contains(_tile))
            {
                Debug.LogWarning("Trying to add a tile to region that already contains tile.");
                return;
            }

            Tiles.Add(_tile);
            _tile.Region = this;
        }
    }
}
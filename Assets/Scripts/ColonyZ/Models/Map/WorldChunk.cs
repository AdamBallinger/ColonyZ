using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map
{
    public struct WorldChunk
    {
        public int X { get; }

        public int Y { get; }

        public HashSet<Tile> Tiles { get; }

        public WorldChunk(int _x, int _y)
        {
            X = _x;
            Y = _y;
            Tiles = new HashSet<Tile>();
        }

        public void Add(Tile _tile)
        {
            if (Tiles.Contains(_tile)) return;

            Tiles.Add(_tile);
        }

        public bool Contains(Tile _tile)
        {
            return Tiles.Contains(_tile);
        }
    }
}
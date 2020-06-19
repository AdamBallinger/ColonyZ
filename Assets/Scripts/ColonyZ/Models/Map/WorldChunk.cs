using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map
{
    public class WorldChunk
    {
        public int X { get; }

        public int Y { get; }

        public HashSet<Tile> Tiles { get; }

        /// <summary>
        /// Determines if the chunk has been modified this frame and should be rebuilt at the end.
        /// </summary>
        public bool IsDirty { get; private set; }

        public WorldChunk(int _x, int _y)
        {
            X = _x;
            Y = _y;
            Tiles = new HashSet<Tile>();
            IsDirty = false;
        }

        public void SetDirty(bool _dirty)
        {
            IsDirty = _dirty;
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

        public override string ToString()
        {
            return $"Chunk X: {X} Y:{Y} Dirty: {IsDirty}";
        }
    }
}
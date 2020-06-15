using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map.Pathing
{
    public class Path
    {
        // https://pastebin.com/vMe9464D
        private int currentIndex;

        public bool IsValid { get; private set; }

        /// <summary>
        ///     Returns the time in milliseconds taken to compute the path.
        /// </summary>
        public float ComputeTime { get; }

        public int Size => TilePath.Count;

        public Tile CurrentTile => TilePath[currentIndex];

        /// <summary>
        ///     The list of tiles in the path.
        /// </summary>
        public List<Tile> TilePath { get; }

        public bool LastTile => currentIndex == Size - 1;

        private List<Node> Nodes { get; }

        public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
        {
            TilePath = new List<Tile>();
            IsValid = _isValid;
            ComputeTime = _computeTime;
            currentIndex = 0;

            if (IsValid)
            {
                Nodes = _nodePath;

                foreach (var node in _nodePath)
                {
                    node.Paths.Add(this);
                    TilePath.Add(World.Instance.GetTileAt(node.X, node.Y));
                }
            }
        }

        public void Next()
        {
            Nodes[currentIndex].Paths.Remove(this);
            currentIndex++;
        }

        public void Invalidate()
        {
            IsValid = false;
        }
    }
}
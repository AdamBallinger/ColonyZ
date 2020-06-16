using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;
using UnityEngine;

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

        public List<Vector2> VectorPath { get; }

        public Vector2 Current => VectorPath[currentIndex];

        public int Size => VectorPath.Count;

        public bool IsLastPoint => currentIndex == Size - 1;

        /// <summary>
        ///     The list of tiles in the path.
        /// </summary>
        private List<Tile> TilePath { get; }

        private List<Node> Nodes { get; }

        public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
        {
            VectorPath = new List<Vector2>();
            IsValid = _isValid;
            ComputeTime = _computeTime;
            currentIndex = 0;

            if (IsValid)
            {
                Nodes = _nodePath;

                foreach (var node in _nodePath)
                {
                    node.Paths.Add(this);
                    VectorPath.Add(new Vector2(node.X, node.Y));
                }

                // TODO: Smooth vector path using Catmullrom.
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
using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map.Pathing
{
    public class Path
    {
        /// <summary>
        ///     The number of additional points to add in-between each tile in the path.
        /// </summary>
        private const int NUMBER_OF_SMOOTHING_POINTS = 2;

        public bool IsValid { get; private set; }

        /// <summary>
        ///     Returns the time in milliseconds taken to compute the path.
        /// </summary>
        public float ComputeTime { get; }

        /// <summary>
        /// Current point in the path being targeted.
        /// </summary>
        public int CurrentIndex { get; private set; }

        public List<Vector2> SmoothPath { get; }

        /// <summary>
        ///     The current point the path is targeting.
        /// </summary>
        public Vector2 Current => SmoothPath[CurrentIndex];

        public int SmoothSize => SmoothPath.Count;

        public bool IsLastPoint => CurrentIndex >= SmoothSize - 1;

        private int Size => VectorPath.Count;

        private List<Node> Nodes { get; }

        private List<Vector2> VectorPath { get; }

        public Path(List<Node> _nodePath, bool _isValid, float _computeTime, bool _removeStart)
        {
            SmoothPath = new List<Vector2>();
            VectorPath = new List<Vector2>();
            IsValid = _isValid;
            ComputeTime = _computeTime;
            CurrentIndex = 0;

            if (IsValid)
            {
                Nodes = _nodePath;
                
                if (_removeStart && _nodePath.Count > 1) Nodes.RemoveAt(0);

                foreach (var node in Nodes)
                {
                    node.Paths.Add(this);
                    VectorPath.Add(new Vector2(node.Data.X, node.Data.Y));
                }

                const float resolution = 1.0f / NUMBER_OF_SMOOTHING_POINTS;

                for (var i = 0; i < Size; i++)
                {
                    // Don't create any smoothing points at the end of the path.
                    if (i == Size - 1)
                    {
                        SmoothPath.Add(VectorPath[i]);
                        break;
                    }

                    var p0 = GetPointAt(i - 1);
                    var p1 = GetPointAt(i);
                    var p2 = GetPointAt(i + 1);
                    var p3 = GetPointAt(i + 2);

                    for (var t = 0.0f; t < 1.0f; t += resolution)
                    {
                        var pos = GetCatmullRom(t, p0, p1, p2, p3);
                        SmoothPath.Add(pos);
                    }
                }
            }
        }

        public void Next()
        {
            // Only remove path from points that are actually on tiles and not extra smoothing points.
            if (CurrentIndex % NUMBER_OF_SMOOTHING_POINTS == 0)
            {
                Nodes[CurrentIndex / NUMBER_OF_SMOOTHING_POINTS].Paths.Remove(this);
            }

            CurrentIndex++;
        }

        public void Invalidate()
        {
            IsValid = false;

            foreach (var node in Nodes)
            {
                node.Paths.Remove(this);
            }
        }

        private Vector2 GetPointAt(int _index)
        {
            if (_index >= Size)
                return VectorPath[Size - 1];
            return _index < 0 ? VectorPath[0] : VectorPath[_index];
        }

        private Vector2 GetCatmullRom(float _t, Vector2 _p0, Vector2 _p1, Vector2 _p2, Vector2 _p3)
        {
            var a = _p1 * (0.5f * 2.0f);
            var b = 0.5f * (_p2 - _p0);
            var c = 0.5f * (2.0f * _p0 - 5.0f * _p1 + 4.0f * _p2 - _p3);
            var d = 0.5f * (-_p0 + 3.0f * _p1 - 3.0f * _p2 + _p3);

            return a + b * _t + c * (_t * _t) + d * (_t * _t * _t);
        }
    }
}
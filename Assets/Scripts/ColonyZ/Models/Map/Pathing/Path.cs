using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Map.Pathing
{
    public class Path
    {
        /// <summary>
        /// The number of additional points to add in-between each tile in the path.
        /// </summary>
        private const int NUMBER_OF_SMOOTHING_POINTS = 3;

        public bool IsValid { get; private set; }

        /// <summary>
        ///     Returns the time in milliseconds taken to compute the path.
        /// </summary>
        public float ComputeTime { get; }

        public List<Vector2> SmoothPath { get; }

        /// <summary>
        /// The current point the path is targeting.
        /// </summary>
        public Vector2 Current => VectorPath[currentIndex];

        public int SmoothSize => SmoothPath.Count;

        public bool IsLastPoint => currentIndex == Size - 1;

        private int Size => VectorPath.Count;

        private List<Node> Nodes { get; }

        private List<Vector2> VectorPath { get; }

        private int currentIndex;

        public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
        {
            SmoothPath = new List<Vector2>();
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

                const float resolution = 1.0f / NUMBER_OF_SMOOTHING_POINTS;

                for (var i = 0; i < Size; i++)
                {
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

                // Remove the tiny curve generated at the end of a path.
                SmoothPath.RemoveRange(SmoothSize - (NUMBER_OF_SMOOTHING_POINTS + 1), NUMBER_OF_SMOOTHING_POINTS);
            }
        }

        public void Next()
        {
            if (currentIndex % NUMBER_OF_SMOOTHING_POINTS == 0)
            {
                Nodes[currentIndex / NUMBER_OF_SMOOTHING_POINTS].Paths.Remove(this);
            }

            currentIndex++;
        }

        public void Invalidate()
        {
            IsValid = false;
        }

        private Vector2 GetPointAt(int _index)
        {
            if (_index >= Size)
                return VectorPath[Size - 1];
            return _index < 0 ? VectorPath[0] : VectorPath[_index];
        }

        private float GetT(float _t, Vector2 _p0, Vector2 _p1)
        {
            var a = Mathf.Pow(_p1.x - _p0.x, 2.0f) + Mathf.Pow(_p1.y - _p0.y, 2.0f);
            var b = Mathf.Pow(a, 0.5f * 0.5f);

            return b + _t;
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
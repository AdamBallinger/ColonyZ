using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;
using UnityEngine.Profiling;

namespace ColonyZ.Utils
{
    public static class FloodFiller
    {
        private static HashSet<Tile> visited = new HashSet<Tile>();
        private static List<Tile> processed = new List<Tile>();

        private static Queue<Tile> queue = new Queue<Tile>(2000);

        /// <summary>
        ///     Floods a root tile. Check condition determines if the tile can be considered during the flood,
        ///     and passCheck determines if the tile is a part of the flooded area. Successful floods
        ///     are processed by the given processor.
        /// </summary>
        /// <param name="_root"></param>
        /// <param name="_checkCondition"></param>
        /// <param name="_passCheck"></param>
        /// <param name="_processor"></param>
        public static void Flood(Tile _root,
            Predicate<Tile> _checkCondition,
            Predicate<Tile> _passCheck,
            Action<List<Tile>> _processor)
        {
            Profiler.BeginSample("Flood fill sample");
            if (!_checkCondition(_root))
            {
                Profiler.EndSample();
                return;
            }

            visited.Clear();
            processed.Clear();
            queue.Clear();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    processed.Add(current);

                    foreach (var tile in current.DirectNeighbours)
                    {
                        if (!_checkCondition(tile)) continue;

                        if (_passCheck(tile))
                        {
                            queue.Enqueue(tile);
                        }
                        else
                        {
                            Profiler.EndSample();
                            return;
                        }
                    }
                }
            }

            _processor(processed);
            Profiler.EndSample();
        }
    }
}
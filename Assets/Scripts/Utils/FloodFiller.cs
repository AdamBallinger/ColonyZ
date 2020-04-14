using System;
using System.Collections.Generic;
using Models.Map.Tiles;

namespace Utils
{
    public static class FloodFiller
    {
        /// <summary>
        /// Floods a root tile. Check condition determines if the tile can be considered during the flood,
        /// and passCheck determines if the tile is a part of the flooded area. Successful floods
        /// are processed by the given processor.
        /// </summary>
        /// <param name="_root"></param>
        /// <param name="_checkCondition"></param>
        /// <param name="_passCheck"></param>
        /// <param name="_processor"></param>
        public static void Flood(Tile _root,
            Predicate<Tile> _checkCondition,
            Predicate<Tile> _passCheck,
            Action<HashSet<Tile>> _processor)
        {
            if (!_checkCondition(_root))
            {
                return;
            }

            var processed = new HashSet<Tile>();
            var queue = new Queue<Tile>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (!processed.Contains(current))
                {
                    processed.Add(current);

                    foreach (var tile in current.DirectNeighbours)
                    {
                        if (!_checkCondition(tile))
                        {
                            continue;
                        }

                        if (_passCheck(tile))
                        {
                            queue.Enqueue(tile);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }

            _processor(processed);
        }
    }
}
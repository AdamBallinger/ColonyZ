using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;
using Priority_Queue;
using UnityEngine;

namespace ColonyZ.Models.Map.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        public bool UseRegionalPathfinding { get; set; }

        public int TaskCount => taskList.Count;

        private List<Task<PathResult>> taskList;

        private PathFinder()
        {
        }

        /// <summary>
        ///     Creates a new Instance of the PathFinder class.
        /// </summary>
        public static void Create()
        {
            Instance = new PathFinder
            {
                taskList = new List<Task<PathResult>>()
            };
        }

        public static void Destroy()
        {
            Instance = null;
        }

        /// <summary>
        ///     Create a new pathfinding request for the PathFinder. The path will be passed back to the onCompleteCallback given.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        public static void NewRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback)
        {
            Instance?.Queue(new PathRequest(_start, _end, _onCompleteCallback));
        }

        /// <summary>
        ///     Adds a path request to the path finding queue.
        /// </summary>
        /// <param name="_request"></param>
        private void Queue(PathRequest _request)
        {
            taskList.Add(Search(_request));
        }

        /// <summary>
        ///     Process the next path request in the queue.
        /// </summary>
        public async void ProcessNext()
        {
            if (taskList.Count == 0) return;

            var task = await Task.WhenAny(taskList.ToArray());
            taskList.Remove(task);
            var result = task.Result;
            result.InvokeCallback();
        }

        /// <summary>
        ///     Performs A* search for a given path request.
        /// </summary>
        private Task<PathResult> Search(PathRequest _request)
        {
            var sw = new Stopwatch();
            sw.Start();

            PathResult result;

            var regionPath = UseRegionalPathfinding ? GetRegionPath(_request) : null;
            if (UseRegionalPathfinding && regionPath == null)
            {
                sw.Stop();
                result = new PathResult(_request, new Path(null, false, sw.ElapsedMilliseconds));
                return Task.FromResult(result);
            }

            var nodeOpenSet = new FastPriorityQueue<Node>(NodeGraph.Instance.Width * NodeGraph.Instance.Height);
            var nodeClosedSet = new HashSet<Node>();

            var gCosts = new float[World.Instance.Size];
            var hCosts = new float[World.Instance.Size];

            var parents = new Node[World.Instance.Size];

            hCosts[_request.Start.ID] = Heuristic(_request.Start, _request.End);

            nodeOpenSet.Enqueue(_request.Start, hCosts[_request.Start.ID] + gCosts[_request.Start.ID]);

            while (nodeOpenSet.Count != 0)
            {
                var currentNode = nodeOpenSet.Dequeue();

                nodeClosedSet.Add(currentNode);

                if (currentNode == _request.End)
                {
                    sw.Stop();
                    result = new PathResult(_request,
                        new Path(Retrace(currentNode, parents), true, sw.ElapsedMilliseconds));
                    return Task.FromResult(result);
                }

                foreach (var node in currentNode.Neighbours)
                {
                    if (UseRegionalPathfinding)
                    {
                        var tile = World.Instance.GetTileAt(node.X, node.Y);
                        if (!regionPath.Contains(tile.Region)) continue;
                    }

                    if (!node.Pathable || nodeClosedSet.Contains(node)) continue;

                    var movementCostToNeighbour =
                        gCosts[currentNode.ID] + Heuristic(currentNode, node) + node.MovementCost;

                    if (movementCostToNeighbour < gCosts[node.ID] || !nodeOpenSet.Contains(node))
                    {
                        gCosts[node.ID] = movementCostToNeighbour;
                        hCosts[node.ID] = Heuristic(node, _request.End);
                        parents[node.ID] = currentNode;

                        if (!nodeOpenSet.Contains(node))
                            nodeOpenSet.Enqueue(node, gCosts[node.ID] + hCosts[node.ID]);
                        else
                            nodeOpenSet.UpdatePriority(node, gCosts[node.ID] + hCosts[node.ID]);
                    }
                }
            }

            sw.Stop();
            // If every node was evaluated and the end node wasn't found, then invoke the callback with an invalid empty path.
            result = new PathResult(_request, new Path(null, false, sw.ElapsedMilliseconds));
            return Task.FromResult(result);
        }

        private HashSet<Region> GetRegionPath(PathRequest _request)
        {
            var path = new HashSet<Region>();

            var origin = World.Instance.GetTileAt(_request.Start.X, _request.Start.Y).Region;
            var target = World.Instance.GetTileAt(_request.End.X, _request.End.Y).Region;

            if (origin == null || target == null) return null;

            if (origin == target)
            {
                path.Add(origin);
                return path;
            }

            var queue = new Queue<Region>();
            var visited = new HashSet<Region>();
            var parents = new Dictionary<Region, Region>();
            queue.Enqueue(origin);
            visited.Add(origin);
            parents.Add(origin, null);

            while (queue.Count > 0)
            {
                var region = queue.Dequeue();

                if (region == target)
                {
                    path.Add(region);
                    // retrace regions from region using the parent map.
                    while (region != null)
                    {
                        path.Add(parents[region]);
                        region = parents[region];
                    }

                    return path;
                }

                foreach (var link in region.Links)
                {
                    var other = link.GetOther(region);
                    if (other == null) continue;
                    if (visited.Contains(other)) continue;

                    queue.Enqueue(other);
                    visited.Add(other);
                    parents.Add(other, region);
                }
            }

            return path;
        }

        /// <summary>
        ///     Returns a list of nodes from path Start to End.
        /// </summary>
        /// <param name="_lastNode"></param>
        /// <param name="_parents"></param>
        /// <returns></returns>
        private List<Node> Retrace(Node _lastNode, IReadOnlyList<Node> _parents)
        {
            var list = new List<Node>
            {
                _lastNode
            };

            while (_parents[_lastNode.ID] != null)
            {
                list.Add(_parents[_lastNode.ID]);
                _lastNode = _parents[_lastNode.ID];
            }

            list.Reverse();

            return list;
        }

        /// <summary>
        ///     Calculates nodes H cost using Manhattan distance.
        /// </summary>
        /// <param name="_node"></param>
        /// <param name="_end"></param>
        /// <returns></returns>
        private float Heuristic(Node _node, Node _end)
        {
            float dx = Mathf.Abs(_node.X - _end.X);
            float dy = Mathf.Abs(_node.Y - _end.Y);

            //return 1.0f * (dx + dy) + (1.4f - 2.0f * 1.0f) * Mathf.Min(dx, dy);
            return dx + dy;
        }

        private struct PathResult
        {
            private PathRequest request;
            private Path path;

            public PathResult(PathRequest _request, Path _path)
            {
                request = _request;
                path = _path;
            }

            public void InvokeCallback()
            {
                request?.onPathCompleteCallback?.Invoke(path);
            }
        }
    }
}
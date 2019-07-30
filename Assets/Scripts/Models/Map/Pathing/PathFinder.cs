using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Models.Map.Tiles;
using Priority_Queue;
using UnityEngine;

namespace Models.Map.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        public int TaskCount => taskList.Count;
        
        private const float STRAIGHT_MOVEMENT_COST = 1.0f;
        private const float DIAGONAL_MOVEMENT_COST = 1.415746543f;

        private List<Task<PathResult>> taskList;

        private PathFinder() { }

        /// <summary>
        /// Creates a new Instance of the PathFinder class.
        /// </summary>
        public static void Create()
        {
            Instance = new PathFinder
            {
                taskList = new List<Task<PathResult>>()
            };
        }

        /// <summary>
        /// Create a new pathfinding request for the PathFinder. The path will be passed back to the onCompleteCallback given.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        public static void NewRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback)
        {
            Instance?.Queue(new PathRequest(_start, _end, _onCompleteCallback));
        }

        /// <summary>
        /// Adds a path request to the path finding queue.
        /// </summary>
        /// <param name="_request"></param>
        private void Queue(PathRequest _request)
        {
            taskList.Add(Search(_request));
        }

        /// <summary>
        /// Process the next path request in the queue.
        /// </summary>
        public async void ProcessNext()
        {
            if (taskList.Count == 0) return;
            
            var task = await Task.WhenAny(taskList.ToArray());
            taskList.Remove(task);
            var result = task.Result;
            result.request.onPathCompleteCallback?.Invoke(result.path);
        }

        /// <summary>
        /// Performs A* search for a given path request.
        /// </summary>
        private Task<PathResult> Search(PathRequest _request)
        {
            var sw = new Stopwatch();
            sw.Start();

            var pathResult = new PathResult
            {
                request = _request
            };

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
                    pathResult.path = new Path(Retrace(currentNode, parents), true, sw.ElapsedMilliseconds);
                    return Task.FromResult(pathResult);
                }

                foreach(var node in currentNode.Neighbours)
                {
                    if(!node.Pathable || nodeClosedSet.Contains(node))
                    {
                        continue;
                    }

                    var movementCostToNeighbour = gCosts[currentNode.ID] + Heuristic(currentNode, node) + node.MovementCost;

                    if(movementCostToNeighbour < gCosts[node.ID] || !nodeOpenSet.Contains(node))
                    {
                        gCosts[node.ID] = movementCostToNeighbour;
                        hCosts[node.ID] = Heuristic(node, _request.End);
                        parents[node.ID] = currentNode;

                        if(!nodeOpenSet.Contains(node))
                        {
                            nodeOpenSet.Enqueue(node, gCosts[node.ID] + hCosts[node.ID]);
                        }
                        else
                        {
                            nodeOpenSet.UpdatePriority(node, gCosts[node.ID] + hCosts[node.ID]);
                        }
                    }
                }
            }

            // If every node was evaluated and the end node wasn't found, then invoke the callback with an invalid empty path.
            pathResult.path = new Path(null, false, 0.0f);
            return Task.FromResult(pathResult);
        }

        /// <summary>
        /// Returns a list of nodes from path Start to End.
        /// </summary>
        /// <param name="_lastNode"></param>
        /// <param name="_parents"></param>
        /// <returns></returns>
        private IEnumerable<Node> Retrace(Node _lastNode, IReadOnlyList<Node> _parents)
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
        /// Calculates the Diagonal Distance Heuristic cost for a node.
        /// </summary>
        /// <param name="_node"></param>
        /// <param name="_end"></param>
        /// <returns></returns>
        private float Heuristic(Node _node, Node _end)
        {
            var dx = Mathf.Abs(_node.X - _end.X);
            var dy = Mathf.Abs(_node.Y - _end.Y);

            if (dx > dy)
                return DIAGONAL_MOVEMENT_COST * dy + STRAIGHT_MOVEMENT_COST * (dx - dy);

            return DIAGONAL_MOVEMENT_COST * dx + STRAIGHT_MOVEMENT_COST * (dy - dx);
        }
        
        private struct PathResult
        {
            public PathRequest request;
            public Path path;
        }
    }
}

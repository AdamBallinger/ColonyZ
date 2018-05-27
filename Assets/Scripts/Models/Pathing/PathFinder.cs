using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Models.Map;
using Priority_Queue;
using UnityEngine;

namespace Models.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        private const float STRAIGHT_MOVEMENT_COST = 1.0f;
        private const float DIAGONAL_MOVEMENT_COST = 1.415746543f;

        private SimplePriorityQueue<PathRequest> RequestQueue { get; set; }

        private volatile bool IsBusy;

        private volatile HashSet<Node> NodeClosedSet;
        private volatile FastPriorityQueue<Node> NodeOpenList;

        private volatile Path path;

        private volatile PathRequest currentRequest;

        private volatile bool FoundPath;

        private PathFinder() { }

        /// <summary>
        /// Creates a new Instance of the PathFinder class.
        /// </summary>
        public static void Create()
        {
            Instance = new PathFinder
            {
                RequestQueue = new SimplePriorityQueue<PathRequest>(),
                IsBusy = false,
                NodeClosedSet = new HashSet<Node>(),
                NodeOpenList = new FastPriorityQueue<Node>(NodeGraph.Instance.Width * NodeGraph.Instance.Height)
            };
        }

        /// <summary>
        /// Create a new pathfinding request for the PathFinder. The path will be passed back to the onCompleteCallback given.
        /// An optional parameter for the priority can be given if the path request should be processed with a higher
        /// priority. Lower priority value = quicker processing of request.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        /// <param name="_priority"></param>
        public static void NewRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback, uint _priority = 10)
        {
            Instance?.Queue(new PathRequest(_start, _end, _onCompleteCallback), _priority);
        }

        /// <summary>
        /// Adds a path request to the path finding queue.
        /// </summary>
        /// <param name="_request"></param>
        /// <param name="_priority"></param>
        private void Queue(PathRequest _request, uint _priority)
        {
            RequestQueue.Enqueue(_request, _priority);
        }

        /// <summary>
        /// Process the next path request in the queue.
        /// </summary>
        public async void ProcessNext()
        {
            if (RequestQueue.Count == 0 || IsBusy) return;

            currentRequest = RequestQueue.Dequeue();

            if (currentRequest != null)
            {
                IsBusy = true;
                await Task.Run(() => Search());
                currentRequest.onPathCompleteCallback?.Invoke(path);
                IsBusy = false;
                currentRequest = null;
            }
        }

        /// <summary>
        /// Performs A* search for a given path request.
        /// </summary>
        private void Search()
        {
            var sw = new Stopwatch();
            sw.Start();

            FoundPath = false;

            NodeClosedSet.Clear();
            NodeOpenList.Clear();

            currentRequest.Start.H = Heuristic(currentRequest.Start, currentRequest.End);

            NodeOpenList.Enqueue(currentRequest.Start, currentRequest.Start.F);

            while (NodeOpenList.Count != 0)
            {
                var currentNode = NodeOpenList.Dequeue();

                NodeClosedSet.Add(currentNode);

                if (currentNode == currentRequest.End)
                {
                    sw.Stop();
                    FoundPath = true;
                    path = new Path(Retrace(currentNode), true, sw.ElapsedMilliseconds);
                    break;
                }

                foreach(var node in currentNode.Neighbours)
                {
                    if(!node.Pathable || NodeClosedSet.Contains(node) /*|| !nodesToEvaluate.Contains(node)*/)
                        continue;

                    var movementCostToNeigbour = currentNode.G + Heuristic(currentNode, node) + node.MovementCost;

                    if(movementCostToNeigbour < node.G || !NodeOpenList.Contains(node))
                    {
                        node.G = movementCostToNeigbour;
                        node.H = Heuristic(node, currentRequest.End);
                        node.Parent = currentNode;

                        if(!NodeOpenList.Contains(node))
                            NodeOpenList.Enqueue(node, node.F);
                        else
                            NodeOpenList.UpdatePriority(node, node.F);
                    }
                }
            }

            // If every node was evaluated and the end node wasn't found, then invoke the callback with an invalid empty path.
            if (!FoundPath)
            {
                path = new Path(null, false, 0.0f);
            }

            foreach(var node in NodeClosedSet)
            {
                node.Reset();
            }

            foreach(var node in NodeOpenList)
            {
                node.Reset();
            }
        }

        /// <summary>
        /// Returns a list of nodes from path Start to End.
        /// </summary>
        /// <param name="_lastNode"></param>
        /// <returns></returns>
        private List<Node> Retrace(Node _lastNode)
        {
            var list = new List<Node>
            {
                _lastNode
            };

            while (_lastNode.Parent != null)
            {
                list.Add(_lastNode.Parent);
                _lastNode = _lastNode.Parent;
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
    }
}

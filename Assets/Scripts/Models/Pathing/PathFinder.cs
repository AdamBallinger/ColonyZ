using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Models.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        private Queue<PathRequest> RequestQueue { get; set; }

        private volatile bool IsBusy;

        private HashSet<Node> ClosedList { get; set; }
        private List<Node> OpenList { get; set; }

        private volatile Path path;

        private bool FoundPath { get; set; }

        private PathFinder() { }

        /// <summary>
        /// Creates a new Instance of the PathFinder class.
        /// </summary>
        public static void Create()
        {
            Instance = new PathFinder
            {
                RequestQueue = new Queue<PathRequest>(),
                IsBusy = false,
                ClosedList = new HashSet<Node>(),
                OpenList = new List<Node>()
            };
        }

        /// <summary>
        /// Adds a path request to the path finding queue.
        /// </summary>
        /// <param name="_request"></param>
        public void Queue(PathRequest _request)
        {
            RequestQueue.Enqueue(_request);
        }

        /// <summary>
        /// Process the next path request in the queue.
        /// </summary>
        public async void ProcessNext()
        {
            if (RequestQueue.Count == 0 || IsBusy) return;

            var request = RequestQueue.Dequeue();

            if (request != null)
            {
                IsBusy = true;
                await Task.Run(() => Search(request));
                request.onPathCompleteCallback.Invoke(path);
                IsBusy = false;
            }
        }

        /// <summary>
        /// Performs A* search for a given path request in a seperate thread.
        /// </summary>
        /// <param name="_request"></param>
        private void Search(PathRequest _request)
        {
            var sw = new Stopwatch();
            sw.Start();

            FoundPath = false;

            ClosedList.Clear();
            OpenList.Clear();

            OpenList.Add(_request.Start);

            _request.Start.H = Heuristic(_request.Start, _request.End);

            while (OpenList.Count > 0)
            {
                var currentNode = GetLowestFCostNode();

                OpenList.Remove(currentNode);
                ClosedList.Add(currentNode);

                if (currentNode == _request.End)
                {
                    sw.Stop();
                    FoundPath = true;
                    path = new Path(Retrace(currentNode), true, sw.ElapsedMilliseconds);
                    break;
                }

                foreach(var node in currentNode.Neighbours)
                {
                    if(!node.Pathable || ClosedList.Contains(node))
                        continue;

                    var movementCostToNeigbour = currentNode.G + Heuristic(currentNode, node) + node.MovementCost;

                    if(movementCostToNeigbour < node.G || !OpenList.Contains(node))
                    {
                        node.G = movementCostToNeigbour;
                        node.H = Heuristic(node, _request.End);
                        node.Parent = currentNode;

                        if(!OpenList.Contains(node))
                            OpenList.Add(node);
                    }
                }
            }

            // If every node was evaluated and the end node wasn't found, then invoke the callback with an invalid empty path.
            if (!FoundPath)
            {
                path = new Path(null, false, 0.0f);
            }
        }

        /// <summary>
        /// Gets the node with the lowest F cost from the OpenList.
        /// </summary>
        /// <returns></returns>
        private Node GetLowestFCostNode()
        {
            var cheapestNode = OpenList[0];

            foreach (var node in OpenList)
            {
                if (node.F < cheapestNode.F && node.Pathable)
                {
                    cheapestNode = node;
                }
            }

            return cheapestNode;
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
                return 14.0f * dy + 10.0f * (dx - dy);

            return 14.0f * dx + 10.0f * (dy - dx);
        }
    }
}

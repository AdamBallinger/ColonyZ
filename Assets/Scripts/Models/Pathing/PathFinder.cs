﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Priority_Queue;
using UnityEngine;

namespace Models.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        private const float STRAIGHT_MOVEMENT_COST = 10.0f;
        private const float DIAGONAL_MOVEMENT_COST = 14.0f;

        private Queue<PathRequest> RequestQueue { get; set; }

        private volatile bool IsBusy;

        private HashSet<NodeChunk> ChunkClosedSet { get; set; }
        private FastPriorityQueue<NodeChunk> ChunkOpenList { get; set; }

        private HashSet<Node> NodeClosedSet { get; set; }
        private FastPriorityQueue<Node> NodeOpenList { get; set; }

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
                ChunkClosedSet = new HashSet<NodeChunk>(),
                ChunkOpenList = new FastPriorityQueue<NodeChunk>(NodeGraph.Instance.Width / NodeGraph.Instance.ChunkSize * (NodeGraph.Instance.Height / NodeGraph.Instance.ChunkSize)),
                NodeClosedSet = new HashSet<Node>(),
                NodeOpenList = new FastPriorityQueue<Node>(NodeGraph.Instance.Width * NodeGraph.Instance.Height)
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

            NodeClosedSet.Clear();
            NodeOpenList.Clear();

            //var nodesToEvaluate = ChunkSearch(_request);

            //if(nodesToEvaluate == null)
            //{
            //    path = new Path(null, false, 0.0f);
            //    return;
            //}

            _request.Start.H = Heuristic(_request.Start, _request.End);

            NodeOpenList.Enqueue(_request.Start, _request.Start.F);

            while (NodeOpenList.Count != 0)
            {
                var currentNode = NodeOpenList.Dequeue();

                NodeClosedSet.Add(currentNode);

                if (currentNode == _request.End)
                {
                    sw.Stop();
                    FoundPath = true;
                    path = new Path(Retrace(currentNode), true, sw.ElapsedMilliseconds);
                    break;
                }

                foreach(var node in currentNode.Neighbours)
                {
                    if(!node.Pathable || NodeClosedSet.Contains(node)/* || !nodesToEvaluate.Contains(node)*/)
                        continue;

                    var movementCostToNeigbour = currentNode.G + Heuristic(currentNode, node) + node.MovementCost;

                    if(movementCostToNeigbour < node.G || !NodeOpenList.Contains(node))
                    {
                        node.G = movementCostToNeigbour;
                        node.H = Heuristic(node, _request.End);
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
        }

        private List<Node> ChunkSearch(PathRequest _request)
        {
            ChunkClosedSet.Clear();
            ChunkOpenList.Clear();

            var startChunk = NodeGraph.Instance.GetChunkInWorld(_request.Start);
            var endChunk = NodeGraph.Instance.GetChunkInWorld(_request.End);

            startChunk.H = Heuristic(startChunk, endChunk);

            ChunkOpenList.Enqueue(startChunk, startChunk.F);

            while(ChunkOpenList.Count > 0)
            {
                var currentChunk = ChunkOpenList.Dequeue();

                ChunkClosedSet.Add(currentChunk);

                if(currentChunk == endChunk)
                {
                    UnityEngine.Debug.Log("Found chunk route.");
                    return RetraceChunks(currentChunk);
                }

                var neighbours = currentChunk.GetNeighbours();

                foreach(var chunk in neighbours)
                {
                    if (ChunkClosedSet.Contains(chunk))
                        continue;

                    var movementCostToNeigbour = currentChunk.G + Heuristic(currentChunk, chunk);

                    if (movementCostToNeigbour < chunk.G || !ChunkOpenList.Contains(chunk))
                    {        
                        chunk.G = movementCostToNeigbour;
                        chunk.H = Heuristic(chunk, endChunk);
                        chunk.Parent = currentChunk;

                        if (!ChunkOpenList.Contains(chunk))
                            ChunkOpenList.Enqueue(chunk, chunk.F);
                        else
                            ChunkOpenList.UpdatePriority(chunk, chunk.F);
                    }
                }
            }

            UnityEngine.Debug.LogWarning("Failed to find a chunk route.");
            return null;
        }

        private List<Node> RetraceChunks(NodeChunk _lastChunk)
        {
            var list = _lastChunk.Nodes.Cast<Node>().ToList();

            while (_lastChunk.Parent != null)
            {
                list.AddRange(_lastChunk.Parent.Nodes.Cast<Node>().ToList());
                _lastChunk = _lastChunk.Parent;
            }

            return list;
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

        private float Heuristic(NodeChunk _chunk, NodeChunk _end)
        {
            var dx = Mathf.Abs(_chunk.X - _end.X);
            var dy = Mathf.Abs(_chunk.Y - _end.Y);

            if (dx > dy)
                return DIAGONAL_MOVEMENT_COST * dy + STRAIGHT_MOVEMENT_COST * (dx - dy);

            return DIAGONAL_MOVEMENT_COST * dx + STRAIGHT_MOVEMENT_COST * (dy - dx);
        }
    }
}

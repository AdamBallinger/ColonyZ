using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ColonyZ.Models.Map.Pathing
{
    public class PathFinder
    {
        public static PathFinder Instance { get; private set; }

        /// <summary>
        ///     Number of path requests waiting to be processed.
        /// </summary>
        public int RequestCount => requestQueue.Count;
        
        private Queue<PathRequest> requestQueue;

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
                requestQueue = new Queue<PathRequest>()
            };
        }

        public static void Destroy()
        {
            Instance?.Dispose();
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
            requestQueue.Enqueue(_request);
        }
        
        public void Update()
        {
            var tempGraph = new NativeArray<NodeData>(NodeGraph.Instance.NodeData, Allocator.TempJob);
            
            if (requestQueue.Count > 0)
            {
                var request = requestQueue.Dequeue();

                var jobData = new PathFinderJob
                {
                    startID = request.Start.Data.ID,
                    endID = request.End.Data.ID,
                    gridSize = new int2(NodeGraph.Instance.Width, NodeGraph.Instance.Height),
                    graph = tempGraph,
                    openSet = new NativeList<int>(Allocator.TempJob),
                    closedSet = new NativeList<int>(Allocator.TempJob),
                    path = new NativeList<int>(Allocator.TempJob),
                    valid = new NativeArray<bool>(1, Allocator.TempJob)
                };
                
                jobData.Schedule().Complete();

                if (!jobData.valid[0])
                {
                    request.onPathCompleteCallback?.Invoke(null);
                }
                else
                {
                    var result = jobData.path;
                    var list = new List<Node>();
                    for (var i = result.Length - 1; i >= 0; i--)
                    {
                        list.Add(NodeGraph.Instance.GetNodeAt(result[i]));
                    }
                    request.onPathCompleteCallback?.Invoke(new Path(list, true, 0.0f));
                }

                jobData.openSet.Dispose();
                jobData.closedSet.Dispose();
                jobData.path.Dispose();
                jobData.valid.Dispose();
            }

            tempGraph.Dispose();
        }

        private void Dispose()
        {

        }
    }
}
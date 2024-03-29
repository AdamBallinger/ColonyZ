﻿using System;
using System.Collections.Generic;
using ColonyZ.Models.Map.Tiles;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

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

        private NativeList<JobHandle> handles = new NativeList<JobHandle>(Allocator.Persistent);
        private List<PathFinderJob> jobs = new List<PathFinderJob>();
        private List<PathRequest> requests = new List<PathRequest>();

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
        /// <param name="_removeStart"></param>
        public static void NewRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback, bool _removeStart = true)
        {
            Instance?.Queue(new PathRequest(_start, _end, _onCompleteCallback, _removeStart));
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
            if (requestQueue.Count > 0)
            {
                var request = requestQueue.Dequeue();
                requests.Add(request);

                var jobData = new PathFinderJob
                {
                    startID = request.Start.Data.ID,
                    endID = request.End.Data.ID,
                    gridSize = new int2(NodeGraph.Instance.Width, NodeGraph.Instance.Height),
                    startTime = Time.unscaledTime,
                    graph = new NativeArray<NodeData>(NodeGraph.Instance.NodeData, Allocator.Persistent),
                    openSet = new NativeList<int>(Allocator.Persistent),
                    closedSet = new NativeList<int>(Allocator.Persistent),
                    path = new NativeList<int>(Allocator.Persistent),
                    valid = new NativeArray<bool>(1, Allocator.Persistent)
                };
                
                handles.Add(jobData.Schedule());
                jobs.Add(jobData);
            }

            for (var i = handles.Length - 1; i >= 0; i--)
            {
                var handle = handles[i];
                if (handle.IsCompleted)
                {
                    handle.Complete();
                    var job = jobs[i];
                    var request = requests[i];

                    if (!job.valid[0])
                    {
                        request.onPathCompleteCallback?.Invoke(null);
                    }
                    else
                    {
                        var result = job.path;
                        var time = (float)Math.Round(Time.unscaledTime - job.startTime, 2);
                        var list = new List<Node>();
                        for (var j = result.Length - 1; j >= 0; j--)
                        {
                            list.Add(NodeGraph.Instance.GetNodeAt(result[j]));
                        }
                        request.onPathCompleteCallback?.Invoke(new Path(list, true, time, request.RemoveStart));
                    }

                    job.graph.Dispose();
                    job.openSet.Dispose();
                    job.closedSet.Dispose();
                    job.path.Dispose();
                    job.valid.Dispose();
                    
                    jobs.RemoveAt(i);
                    handles.RemoveAt(i);
                    requests.RemoveAt(i);
                }
            }
        }

        private void Dispose()
        {
            for (var i = handles.Length - 1; i >= 0; i--)
            {
                var handle = handles[i];
                var job = jobs[i];
                handle.Complete();
                job.graph.Dispose();
                job.openSet.Dispose();
                job.closedSet.Dispose();
                job.path.Dispose();
                job.valid.Dispose();
            }
            
            handles.Dispose();
        }
    }
}
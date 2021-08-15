﻿using System;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map.Pathing
{
    public struct PathRequest
    {
        /// <summary>
        ///     Callback function invoked when the path has been computed.
        /// </summary>
        public Action<Path> onPathCompleteCallback;

        /// <summary>
        ///     The starting Node for the path request.
        /// </summary>
        public Node Start { get; }

        /// <summary>
        ///     The end node the path request will attempt to generate a path to from start.
        /// </summary>
        public Node End { get; }

        /// <summary>
        ///     Create a new path request that will generate a path from start to end and pass it
        ///     back to the given callback function when the path has been completed.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        public PathRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback)
        {
            Start = NodeGraph.Instance.GetNodeAt(_start.X, _start.Y);
            End = NodeGraph.Instance.GetNodeAt(_end.X, _end.Y);
            onPathCompleteCallback = _onCompleteCallback;

            // If the end node is not a pathable node, then just ignore this request and return an empty callback.
            if (End.Data.ID == -1 || !End.Pathable)
            {
                onPathCompleteCallback?.Invoke(new Path(null, false, -1.0f));
            }
        }
    }
}
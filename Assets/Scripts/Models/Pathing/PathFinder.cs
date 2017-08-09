using System;
using Models.Map;

namespace Models.Pathing
{
	public class PathFinder
	{

        /// <summary>
        /// The starting Node for the path finder.
        /// </summary>
        private Node Start { get; }

        /// <summary>
        /// The end node the path finder will attempt to generate a path to from start.
        /// </summary>
        private Node End { get; }

        /// <summary>
        /// Callback function invoked when the path has been computed.
        /// </summary>
	    private Action<Path> onPathCompleteCallback;

        /// <summary>
        /// Create a new pathfinder that will generate a path from start to end and pass it back to the given callback function when
        /// the path has been completed.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        public PathFinder(Tile _start, Tile _end, Action<Path> _onCompleteCallback)
        {
            Start = NodeGraph.Instance?.Nodes[_start.X, _start.Y];
            End = NodeGraph.Instance?.Nodes[_end.X, _end.Y];

            onPathCompleteCallback += _onCompleteCallback;

            // TODO: Threaded path finding with some sort of queue for paths. Only 1 path can be computed at any given time...
        }
	}
}

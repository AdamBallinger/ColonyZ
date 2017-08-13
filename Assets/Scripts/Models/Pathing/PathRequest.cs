using Models.Map;
using System;

namespace Models.Pathing
{
    public class PathRequest
	{

        /// <summary>
        /// The starting Node for the path request.
        /// </summary>
        public Node Start { get; }

        /// <summary>
        /// The end node the path request will attempt to generate a path to from start.
        /// </summary>
        public Node End { get; }

        /// <summary>
        /// Callback function invoked when the path has been computed.
        /// </summary>
	    public Action<Path> onPathCompleteCallback;

        /// <summary>
        /// Create a new path request that will generate a path from start to end and pass it back to the given callback function when
        /// the path has been completed.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        /// <param name="_onCompleteCallback"></param>
        public PathRequest(Tile _start, Tile _end, Action<Path> _onCompleteCallback)
        {
            Start = NodeGraph.Instance?.GetNodeAt(_start.X, _start.Y);
            End = NodeGraph.Instance?.GetNodeAt(_end.X, _end.Y);

            // If the start or end node is not a pathable node, then just ignore this request and return an empty callback.
            if (!Start.Pathable || !End.Pathable)
            {
                _onCompleteCallback?.Invoke(new Path(null, false, -1.0f));
                return;
            }

            onPathCompleteCallback += _onCompleteCallback;

            // TODO: Maybe this should be added to a function to allow control over when the path is queued?
            //PathFinder.Instance?.Queue(this);
        }
	}
}

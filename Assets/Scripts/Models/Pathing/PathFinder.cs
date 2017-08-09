using System;

namespace Models.Pathing
{
	public class PathFinder
	{

        /// <summary>
        /// Callback function invoked when the path has been computed.
        /// </summary>
	    private Action<Path> onPathCompleteCallback;

        public PathFinder(Action<Path> _onCompleteCallback)
        {
            onPathCompleteCallback += _onCompleteCallback;

            // TODO: Threaded path finding with some sort of queue for paths. Only 1 path can be computed at any given time...
        }
	}
}

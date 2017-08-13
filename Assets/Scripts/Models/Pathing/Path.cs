using System.Collections.Generic;

namespace Models.Pathing
{
	public class Path
	{
	    public List<Node> NodePath { get; }

	    public Node EndNode => NodePath?[NodePath.Count - 1];

        public bool IsValid { get; }

        public float ComputeTime { get; }

		public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
		{
		    NodePath = _nodePath;
		    IsValid = _isValid;
		    ComputeTime = _computeTime;
		}
	}
}

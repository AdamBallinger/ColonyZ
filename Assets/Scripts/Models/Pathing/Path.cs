using System.Collections.Generic;

namespace Models.Pathing
{
	public class Path
	{
	    public List<Node> NodePath { get; }

        public bool IsValid { get; }

		public Path(List<Node> _nodePath, bool _isValid)
		{
		    NodePath = _nodePath;
		    IsValid = _isValid;
		}
	}
}

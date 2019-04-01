using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Pathing
{
	public class Path
	{
	    public bool IsValid { get; }

	    public float ComputeTime { get; }

	    public Tile EndTile => World.Instance?.GetTileAt(NodePath[NodePath.Count - 1].X, NodePath[NodePath.Count - 1].Y);

	    public int Size => NodePath.Count;

	    public List<Tile> TilePath
	    {
	        get
	        {
	            tilePath.Clear();
                
                foreach(var node in NodePath)
                {
                    tilePath.Add(World.Instance?.GetTileAt(node.X, node.Y));
                }

	            return tilePath;
	        }
	    }

        private List<Node> NodePath { get; }
	    private List<Tile> tilePath;

		public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
		{
		    NodePath = _nodePath;
		    IsValid = _isValid;
		    ComputeTime = _computeTime;

            tilePath = new List<Tile>();
		}

        /// <summary>
        /// Gets the next tile in the path.
        /// </summary>
        /// <returns></returns>
        public Tile Next()
        {
            if(NodePath != null && NodePath.Count > 0)
            {
                var nextTile = World.Instance?.GetTileAt(NodePath[0].X, NodePath[0].Y);
                NodePath.RemoveAt(0);
                return nextTile;
            }

            return null;
        }
	}
}

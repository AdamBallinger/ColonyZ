using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Pathing
{
	public class Path
	{
	    public bool IsValid { get; private set; }

	    /// <summary>
	    /// Returns the time in milliseconds taken to compute the path.
	    /// </summary>
	    public float ComputeTime { get; }
	    
	    public int Size => TilePath.Count;
	    
	    /// <summary>
	    /// The list of remaining tiles in the path.
	    /// </summary>
	    public List<Tile> TilePath { get; }

	    /// <summary>
	    /// Returns the current tile at the start of the path list.
	    /// </summary>
	    public Tile CurrentTile { get; private set; }
	    
	    private List<Node> Nodes { get; }

	    public Path(List<Node> _nodePath, bool _isValid, float _computeTime)
		{
			TilePath = new List<Tile>();
			IsValid = _isValid;
		    ComputeTime = _computeTime;

		    if (IsValid)
		    {
			    Nodes = _nodePath;
			    
			    foreach (var node in _nodePath)
			    {
				    node.Paths.Add(this);
				    TilePath.Add(World.Instance.GetTileAt(node.X, node.Y));
			    }

			    Nodes[0].Paths.Remove(this);

			    // Remove the starting tile as it the tile that an entity will start from.
			    Nodes.RemoveAt(0);
			    TilePath.RemoveAt(0);
			    Next();
		    }
		}

        /// <summary>
        /// Sets the current tile to the first tile of the path then removes it.
        /// </summary>
        public void Next()
        {
            if(Size > 0)
            {
	            CurrentTile = TilePath[0];
	            Nodes[0].Paths.Remove(this);
	            Nodes.RemoveAt(0);
	            TilePath.RemoveAt(0);
	            return;
            }

            CurrentTile = null;
        }
        
        public void Invalidate()
        {
	        IsValid = false;
        }
	}
}

using System.Collections.Generic;

namespace Models.Pathing
{
	public class Node 
	{
        /// <summary>
        /// X position of the Node in the node graph.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y position of the Node in the node graph.
        /// </summary>
        public int Y { get; }

        public float H { get; set; }
        public float G { get; set; }

	    public float F => H + G;

        public bool Pathable { get; }

        public Node Parent { get; set; }

        /// <summary>
        /// A list of all neighbouring nodes adjacent to this node.
        /// </summary>
        public List<Node> Neighbours { get; set; }

        public Node(int _x, int _y, bool _pathable)
        {
            X = _x;
            Y = _y;
            H = 0;
            G = 0;
            Pathable = _pathable;
            Parent = null;
        }

        /// <summary>
        /// Populates the neighbours list for this Node.
        /// </summary>
        public void ComputeNeighbours()
        {
            
        }
	}
}

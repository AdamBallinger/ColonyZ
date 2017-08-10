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

	    public float MovementCost { get; }

        public bool Pathable { get; }

        public Node Parent { get; set; }

        /// <summary>
        /// A list of all neighbouring nodes adjacent to this node.
        /// </summary>
        public List<Node> Neighbours { get; set; }

        public Node(int _x, int _y, float __movementCost, bool _pathable)
        {
            X = _x;
            Y = _y;
            H = 0;
            G = 0;
            MovementCost = __movementCost;
            Pathable = _pathable;
            Parent = null;
            Neighbours = new List<Node>();
        }

        /// <summary>
        /// Populates the neighbours list for this Node.
        /// </summary>
        public void ComputeNeighbours()
        {
            Neighbours.Clear();

            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X, Y + 1));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X + 1, Y));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X, Y - 1));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X - 1, Y));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X + 1, Y + 1));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X + 1, Y - 1));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X - 1, Y - 1));
            Neighbours.Add(NodeGraph.Instance?.GetNodeAt(X - 1, Y + 1));

            // Remove any null nodes from list.
            Neighbours.RemoveAll(node => node == null);
        }
	}
}

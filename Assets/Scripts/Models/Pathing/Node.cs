using System.Collections.Generic;
using Priority_Queue;

namespace Models.Pathing
{
	public class Node : FastPriorityQueueNode
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

        /// <summary>
        /// The cost of moving into this node.
        /// </summary>
	    public float MovementCost { get; }

	    private bool _pathable = true;
	    /// <summary>
	    /// Is this node a pathable node?
	    /// </summary>
	    public bool Pathable
	    {
	        get { return _pathable; }
            set
            {
                if(value != _pathable)
                {
                    _pathable = value;
                    OnModify();
                }
            }
	    }

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
        /// Updates the neighbours list for this Node.
        /// </summary>
        public void ComputeNeighbours()
        {
            Neighbours.Clear();

            var node_N = NodeGraph.Instance?.GetNodeAt(X, Y + 1);
            var node_E = NodeGraph.Instance?.GetNodeAt(X + 1, Y);
            var node_S = NodeGraph.Instance?.GetNodeAt(X, Y - 1);
            var node_W = NodeGraph.Instance?.GetNodeAt(X - 1, Y);

            Neighbours.Add(node_N);
            Neighbours.Add(node_E);
            Neighbours.Add(node_S);
            Neighbours.Add(node_W);

            var node_NE = NodeGraph.Instance?.GetNodeAt(X + 1, Y + 1);
            var node_SE = NodeGraph.Instance?.GetNodeAt(X + 1, Y - 1);
            var node_SW = NodeGraph.Instance?.GetNodeAt(X - 1, Y - 1);
            var node_NW = NodeGraph.Instance?.GetNodeAt(X - 1, Y + 1);

            if(node_N != null && node_N.Pathable && node_E != null && node_E.Pathable)
            {
                Neighbours.Add(node_NE);
            }

            if(node_N != null && node_N.Pathable && node_W != null && node_W.Pathable)
            {
                Neighbours.Add(node_NW);
            }

            if(node_S != null && node_S.Pathable && node_E != null && node_E.Pathable)
            {
                Neighbours.Add(node_SE);
            }

            if (node_S != null && node_S.Pathable && node_W != null && node_W.Pathable)
            {
                Neighbours.Add(node_SW);
            }

            // Remove any null and none pathable nodes from list.
            Neighbours.RemoveAll(node => node == null);
            Neighbours.RemoveAll(node => !node.Pathable);
        }

        /// <summary>
        /// Called when the Pathable property for this node has changed.
        /// </summary>
        private void OnModify()
        {
            ComputeNeighbours();
            NotifyNeighboursToUpdate();
        }

        public void Reset()
        {
            Parent = null;
            G = 0.0f;
            H = 0.0f;
        }

        private void NotifyNeighboursToUpdate()
        {
            foreach(var node in Neighbours)
            {
                node?.ComputeNeighbours();
            }
        }
	}
}

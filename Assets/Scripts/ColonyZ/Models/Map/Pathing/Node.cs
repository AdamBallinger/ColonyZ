using System.Collections.Generic;

namespace ColonyZ.Models.Map.Pathing
{
    public class Node
    {
        /// <summary>
        ///     Data containing ID, position and pathable state for this node.
        /// </summary>
        public NodeData Data;

        /// <summary>
        ///     List of paths that contain this node.
        /// </summary>
        public List<Path> Paths { get; }

        /// <summary>
        ///     Is this node a pathable node?
        /// </summary>
        public bool Pathable => Data.Pathable;
        
        public bool Accessible { get; set; }
        
        /// <summary>
        ///     List of direct neighbours to this node. (N,E,S,W)
        /// </summary>
        private List<Node> Neighbours { get; }

        public Node(int _id, int _x, int _y, bool _pathable, int _movementCost = 1)
        {
            Data = new NodeData(_id, _x, _y, _pathable, _movementCost);
            Paths = new List<Path>();
            Neighbours = new List<Node>(4);
            Accessible = true;
        }

        public void SetData(NodeData _data)
        {
            if (_data.Pathable != Pathable)
            {
                Data = _data;
                OnModify();
            }
        }

        public void AddNeighbour(Node _node)
        {
            Neighbours.Add(_node);
        }

        /// <summary>
        ///     Called when the Pathable property for this node has changed.
        /// </summary>
        private void OnModify()
        {
            if (!Pathable)
            {
                InvalidatePaths();
            }
            else
            {
                Accessible = true;
            }

            foreach (var node in Neighbours)
            {
                if (node.Pathable) continue;
                var pathableNeighbour = false;
                
                foreach (var n in node.Neighbours)
                {
                    if (n.Pathable)
                    {
                        pathableNeighbour = true;
                        break;
                    }
                }

                node.Accessible = pathableNeighbour;
            }
        }

        /// <summary>
        ///     Notifies any paths on this node that they are no longer valid.
        /// </summary>
        private void InvalidatePaths()
        {
            for (var i = Paths.Count - 1; i >= 0; i--)
            {
                Paths[i].Invalidate();
            }
        }
    }
}
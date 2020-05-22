using System;
using System.Collections.Generic;
using Priority_Queue;

namespace ColonyZ.Models.Map.Pathing
{
    public class Node : FastPriorityQueueNode, IEquatable<Node>
    {
        private bool _pathable = true;

        /// <summary>
        ///     ID of the node in the node graph. Used for fetching node costs during search.
        /// </summary>
        public int ID { get; }

        /// <summary>
        ///     X position of the Node in the node graph.
        /// </summary>
        public int X { get; }

        /// <summary>
        ///     Y position of the Node in the node graph.
        /// </summary>
        public int Y { get; }

        /// <summary>
        ///     The cost of moving into this node.
        /// </summary>
        public float MovementCost { get; }

        /// <summary>
        ///     List of paths that contain this node.
        /// </summary>
        public List<Path> Paths { get; }

        /// <summary>
        ///     Is this node a pathable node?
        /// </summary>
        public bool Pathable
        {
            get => _pathable;
            set
            {
                if (value != _pathable)
                {
                    _pathable = value;
                    OnModify();
                }
            }
        }

        /// <summary>
        ///     A list of all neighbouring nodes adjacent to this node.
        /// </summary>
        public List<Node> Neighbours { get; }

        public Node(int _id, int _x, int _y, float _movementCost, bool _pathable)
        {
            ID = _id;
            X = _x;
            Y = _y;
            MovementCost = _movementCost;
            Paths = new List<Path>();
            Pathable = _pathable;
            Neighbours = new List<Node>();
        }

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _pathable == other._pathable && ID == other.ID && X == other.X && Y == other.Y;
        }

        /// <summary>
        ///     Updates the neighbours list for this Node.
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

            if (node_N != null && node_N.Pathable && node_E != null && node_E.Pathable) Neighbours.Add(node_NE);

            if (node_N != null && node_N.Pathable && node_W != null && node_W.Pathable) Neighbours.Add(node_NW);

            if (node_S != null && node_S.Pathable && node_E != null && node_E.Pathable) Neighbours.Add(node_SE);

            if (node_S != null && node_S.Pathable && node_W != null && node_W.Pathable) Neighbours.Add(node_SW);

            // Remove any null and none pathable nodes from list.
            Neighbours.RemoveAll(node => node == null);
            Neighbours.RemoveAll(node => !node.Pathable);
        }

        /// <summary>
        ///     Called when the Pathable property for this node has changed.
        /// </summary>
        private void OnModify()
        {
            if (!Pathable)
            {
                foreach (var path in Paths) path?.Invalidate();

                Paths.Clear();
            }

            ComputeNeighbours();
            NotifyNeighboursToUpdate();
        }

        private void NotifyNeighboursToUpdate()
        {
            foreach (var node in Neighbours) node?.ComputeNeighbours();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _pathable.GetHashCode();
                hashCode = (hashCode * 397) ^ ID;
                hashCode = (hashCode * 397) ^ X;
                hashCode = (hashCode * 397) ^ Y;
                return hashCode;
            }
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }
    }
}
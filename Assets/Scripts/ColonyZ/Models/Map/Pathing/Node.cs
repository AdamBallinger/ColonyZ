using System.Collections.Generic;

namespace ColonyZ.Models.Map.Pathing
{
    public struct Node
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

        public Node(int _id, int _x, int _y, float _movementCost, bool _pathable)
        {
            Data = new NodeData(_id, _x, _y, _pathable);
            Paths = new List<Path>();
        }

        public void SetData(NodeData _data)
        {
            if (_data.Pathable != Pathable)
            {
                Data = _data;
                OnModify();
            }
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
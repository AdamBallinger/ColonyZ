namespace ColonyZ.Models.Map.Pathing
{
    public struct NodeData
    {
        public int ID { get; }
        public int X { get; }
        public int Y { get; }

        /// <summary>
        ///     Movement penalty when travelling through this node.
        /// </summary>
        public int MovementCost { get; }
        
        public bool Pathable { get; }
        
        public int HCost { get; set; }
        public int GCost { get; set; }
        public int FCost => HCost + GCost;
        
        public int Parent { get; set; }

        public NodeData(int _id, int _x, int _y, bool _pathable, int _movementCost = 1)
        {
            ID = _id;
            X = _x;
            Y = _y;
            MovementCost = _movementCost;
            Pathable = _pathable;
            HCost = int.MaxValue;
            GCost = 0;
            Parent = -1;
        }
    }
}
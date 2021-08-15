namespace ColonyZ.Models.Map.Pathing
{
    public struct NodeData
    {
        public int ID { get; }
        public int X { get; }
        public int Y { get; }

        public bool Pathable { get; set; }
        
        public int HCost { get; set; }
        public int GCost { get; set; }
        public int FCost => HCost + GCost;
        
        public int Parent { get; set; }

        public NodeData(int _id, int _x, int _y, bool _pathable)
        {
            ID = _id;
            X = _x;
            Y = _y;
            Pathable = _pathable;
            HCost = int.MaxValue;
            GCost = 0;
            Parent = -1;
        }
    }
}
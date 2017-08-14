using System.Collections.Generic;
using Models.Map;
using Priority_Queue;

namespace Models.Pathing
{
    public class NodeChunk : FastPriorityQueueNode
    {

        public Node[,] Nodes { get; }

        public int X { get; }

        public int Y { get; }

        public int Size { get; }

        public float G { get; set; } = 0.0f;
        public float H { get; set; } = 0.0f;
        public float F => G + H;

        public NodeChunk Parent { get; set; }

        public NodeChunk(int _x, int _y, int _size)
        {
            Nodes = new Node[_size, _size];

            X = _x;
            Y = _y;
            Size = _size;
        }

        public void AddNode(Node _node, int _x, int _y)
        {
            Nodes[_x, _y] = _node;
        }

        public void UpdateNode(int _x, int _y, bool _pathable)
        {
            if(Nodes[_x, _y].Pathable != _pathable)
            {
                Nodes[_x, _y].Pathable = _pathable;
                Nodes[_x, _y].OnModify();
            }
        }

        public List<NodeChunk> GetNeighbours()
        {
            var neighbours = new List<NodeChunk>();

            var chunk_N = NodeGraph.Instance?.GetChunkAt(X, Y + 1);
            var chunk_E = NodeGraph.Instance?.GetChunkAt(X + 1, Y);
            var chunk_S = NodeGraph.Instance?.GetChunkAt(X, Y - 1);
            var chunk_W = NodeGraph.Instance?.GetChunkAt(X - 1, Y);

            if (chunk_N != null && CheckEnterable(chunk_N, Cardinals.North))
            {
                neighbours.Add(chunk_N);
            }

            if (chunk_E != null && CheckEnterable(chunk_E, Cardinals.East))
            {
                neighbours.Add(chunk_E);
            }

            if (chunk_S != null && CheckEnterable(chunk_S, Cardinals.South))
            {
                neighbours.Add(chunk_S);
            }

            if (chunk_W != null && CheckEnterable(chunk_W, Cardinals.West))
            {
                neighbours.Add(chunk_W);
            }

            var chunk_NE = NodeGraph.Instance?.GetChunkAt(X + 1, Y + 1);
            var chunk_SE = NodeGraph.Instance?.GetChunkAt(X + 1, Y - 1);
            var chunk_SW = NodeGraph.Instance?.GetChunkAt(X - 1, Y - 1);
            var chunk_NW = NodeGraph.Instance?.GetChunkAt(X - 1, Y + 1);

            if (chunk_NE != null && CheckEnterable(chunk_NE, Cardinals.North_East, true))
            {
                neighbours.Add(chunk_NE);
            }

            if (chunk_NW != null && CheckEnterable(chunk_NW, Cardinals.North_West, true))
            {
                neighbours.Add(chunk_NW);
            }

            if (chunk_SE != null && CheckEnterable(chunk_SE, Cardinals.South_East, true))
            {
                neighbours.Add(chunk_SE);
            }

            if (chunk_SW != null && CheckEnterable(chunk_SW, Cardinals.South_West, true))
            {
                neighbours.Add(chunk_SW);
            }

            return neighbours;
        }

        private bool CheckEnterable(NodeChunk _chunkNeighbour, Cardinals _direction, bool _diagonalCheck = false)
        {
            if (!_diagonalCheck)
            {
                switch (_direction)
                {
                    case Cardinals.North:
                        for(var x = 0; x < Size; x++)
                        {
                            if(Nodes[x, Size - 1].Pathable)
                            {
                                if(_chunkNeighbour.Nodes[x, 0].Pathable)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;

                    case Cardinals.South:
                        for (var x = 0; x < Size; x++)
                        {
                            if (Nodes[x, 0].Pathable)
                            {
                                if (_chunkNeighbour.Nodes[x, _chunkNeighbour.Size - 1].Pathable)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;

                    case Cardinals.East:
                        for (var y = 0; y < Size; y++)
                        {
                            if (Nodes[Size - 1, y].Pathable)
                            {
                                if (_chunkNeighbour.Nodes[0, y].Pathable)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;

                    case Cardinals.West:
                        for (var y = 0; y < Size; y++)
                        {
                            if (Nodes[0, y].Pathable)
                            {
                                if (_chunkNeighbour.Nodes[Size - 1, y].Pathable)
                                {
                                    return true;
                                }
                            }
                        }

                        return false;
                }
            }

            Node cornerNodeSelf;
            Node cornerNodeNeighbour;
            Node cornerUpper;
            Node cornerSide;

            switch (_direction)
            {
                case Cardinals.North_East:

                    // check self corners
                    cornerNodeSelf = Nodes[Size - 1, Size - 1];
                    cornerNodeNeighbour = _chunkNeighbour.Nodes[0, 0];

                    if (!cornerNodeSelf.Pathable || !cornerNodeNeighbour.Pathable)
                    {
                        return false;
                    }

                    cornerUpper = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X, cornerNodeSelf.Y + 1);
                    cornerSide = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X + 1, cornerNodeSelf.Y);

                    if (cornerUpper != null && cornerUpper.Pathable || cornerSide != null && cornerSide.Pathable)
                    {
                        return true;
                    }

                    return false;

                case Cardinals.North_West:
                    cornerNodeSelf = Nodes[0, Size - 1];
                    cornerNodeNeighbour = _chunkNeighbour.Nodes[_chunkNeighbour.Size - 1, 0];

                    if (!cornerNodeSelf.Pathable || !cornerNodeNeighbour.Pathable)
                    {
                        return false;
                    }

                    cornerUpper = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X, cornerNodeSelf.Y + 1);
                    cornerSide = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X - 1, cornerNodeSelf.Y);

                    if (cornerUpper != null && cornerUpper.Pathable || cornerSide != null && cornerSide.Pathable)
                    {
                        return true;
                    }

                    return false;

                case Cardinals.South_East:
                    cornerNodeSelf = Nodes[Size - 1, 0];
                    cornerNodeNeighbour = _chunkNeighbour.Nodes[0, _chunkNeighbour.Size - 1];

                    if (!cornerNodeSelf.Pathable || !cornerNodeNeighbour.Pathable)
                    {
                        return false;
                    }

                    cornerUpper = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X, cornerNodeSelf.Y - 1);
                    cornerSide = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X + 1, cornerNodeSelf.Y);

                    if (cornerUpper != null && cornerUpper.Pathable || cornerSide != null && cornerSide.Pathable)
                    {
                        return true;
                    }

                    return false;

                case Cardinals.South_West:
                    cornerNodeSelf = Nodes[0, 0];
                    cornerNodeNeighbour = _chunkNeighbour.Nodes[_chunkNeighbour.Size - 1, _chunkNeighbour.Size - 1];

                    if (!cornerNodeSelf.Pathable || !cornerNodeNeighbour.Pathable)
                    {
                        return false;
                    }

                    cornerUpper = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X, cornerNodeSelf.Y - 1);
                    cornerSide = NodeGraph.Instance.GetNodeAt(cornerNodeSelf.X - 1, cornerNodeSelf.Y);

                    if (cornerUpper != null && cornerUpper.Pathable || cornerSide != null && cornerSide.Pathable)
                    {
                        return true; 
                    }

                    return false;
            }

            return false;
        }
    }
}

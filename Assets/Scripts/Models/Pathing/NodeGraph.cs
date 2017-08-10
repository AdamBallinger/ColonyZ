using Models.Map;

namespace Models.Pathing
{
    public class NodeGraph
    {
        public static NodeGraph Instance;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Node[,] Nodes { get; private set; }

        private NodeGraph() { }

        /// <summary>
        /// Creates a new instance of the Node Graph.
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public static void Create(int _width, int _height)
        {
            Instance = new NodeGraph
            {
                Width = _width,
                Height = _height
            };

            Instance.Nodes = new Node[Instance.Width, Instance.Height];

            Instance.BuildFullGraph();
        }

        /// <summary>
        /// Builds the entire Node Graph.
        /// </summary>
        public void BuildFullGraph()
        {
            // Populate Node array.
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    // TODO: Replace 1.0f with an actual movement cost based on tile. For now 1.0 will do.
                    Nodes[x, y] = new Node(x, y, 1.0f, World.Instance.Tiles[x, y].InstalledStructure == null);
                }
            }

            BuildNodeNeighbours();
        }

        /// <summary>
        /// Partially rebuild the node graph from a start and end point with an optional padding for node neighbour computing.
        /// </summary>
        /// <param name="_startX"></param>
        /// <param name="_startY"></param>
        /// <param name="_endX"></param>
        /// <param name="_endY"></param>
        /// <param name="_padding"></param>
        public void BuildPartialGraph(int _startX, int _startY, int _endX, int _endY, int _padding = 2)
        {
            // First rebuild the nodes.
            for (var x = _startX; x <= _endX; x++)
            {
                if (x < 0 || x >= Width) continue;

                for (var y = _startY; y <= _endY; y++)
                {
                    if (y < 0 || y >= Height) continue;

                    // TODO: Replace 1.0f with a real movement cost based on tile. 1.0 will do for now.
                    Nodes[x, y] = new Node(x, y, 1.0f, World.Instance.Tiles[x, y].InstalledStructure == null);
                }
            }

            // Recompute neighbour nodes for the rebuilt nodes, and with some padding to surrounding tiles to allow them to update their own 
            // list of neighbours.
            for (var x = _startX - _padding; x < _endX + _padding; x++)
            {
                if (x < 0 || x >= Width) continue;

                for (var y = _startY - _padding; y < _endY + _padding; y++)
                {
                    if (y < 0 || y >= Height) continue;

                    // Optimisation to ignore neigbour generation for none pathable nodes since they never get
                    // evaluated by the path finder anyway.
                    if (!Nodes[x, y].Pathable) continue;

                    Nodes[x, y].ComputeNeighbours();
                }
            }
        }

        private void BuildNodeNeighbours()
        {
            foreach (var node in Nodes)
            {
                // Ignore none pathable nodes since they wont be evaulated by the path finder.
                if (!node.Pathable) continue;

                node.ComputeNeighbours();
            }
        }

        /// <summary>
        /// Safely get a node at a given X and Y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Node GetNodeAt(int _x, int _y)
        {
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height)
            {
                return null;
            }

            return Nodes[_x, _y];
        }
    }
}

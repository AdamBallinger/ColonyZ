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
            for(var x = 0; x < Width; x++)
            {
                for(var y = 0; y < Height; y++)
                {
                    Nodes[x, y] = new Node(x, y, World.Instance.Tiles[x, y].InstalledStructure == null);
                }
            }

            // Compute node neighbours.
            foreach(var node in Nodes)
            {
                node.ComputeNeighbours();
            }
        }

        /// <summary>
        /// Partially rebuild the node graph around a specific point with a given size.
        /// Given X and Y values are the center of the rebuild point.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_size"></param>
        public void BuildPartialGraph(int _x, int _y, int _size)
        {
            // First rebuild the nodes.
            for(var x = _x - _size / 2; x < _x + _size / 2; x++)
            {
                if (x < 0 || x > Width) continue;

                for(var y = _y - _size / 2; y < _y + _size / 2; y++)
                {
                    if(y < 0 || y > Height) continue;

                    Nodes[x, y] = new Node(x, y, World.Instance.Tiles[x, y].InstalledStructure == null);
                }
            }

            // Next compute their neighbours.
            for (var x = _x - _size / 2; x < _x + _size / 2; x++)
            {
                if (x < 0 || x > Width) continue;

                for (var y = _y - _size / 2; y < _y + _size / 2; y++)
                {
                    if (y < 0 || y > Height) continue;

                    Nodes[x, y].ComputeNeighbours();
                }
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
            if(_x < 0 || _x >= Width - 1 || _y < 0 || _y >= Height - 1)
            {
                return null;
            }

            return Nodes[_x, _y];
        }
	}
}

using System.Diagnostics;
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

            PathFinder.Create();
        }

        /// <summary>
        /// Re-Builds the entire Node Graph.
        /// This should only ever be used once on initialization, and UpdateGraph should be used for updating the graph.
        /// </summary>
        private void BuildFullGraph()
        {
            var sw = new Stopwatch();
            sw.Start();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    Nodes[x, y] = new Node(x, y, 1.0f, World.Instance?.GetTileAt(x, y).InstalledStructure == null);
                }
            }

            BuildNodeNeighbours();
            sw.Stop();
            //UnityEngine.Debug.Log("Graph build time: " + sw.ElapsedMilliseconds + "ms.");
        }

        /// <summary>
        /// Update the node graph around a specified are with an optional padding. By default, padding is 2 nodes.
        /// </summary>
        /// <param name="_startX"></param>
        /// <param name="_startY"></param>
        /// <param name="_endX"></param>
        /// <param name="_endY"></param>
        /// <param name="_padding"></param>
        public void UpdateGraph(int _startX, int _startY, int _endX, int _endY, int _padding = 2)
        {
            var sw = new Stopwatch();
            sw.Start();

            // Iterate each node and update its Pathable property.
            for (var x = _startX; x <= _endX; x++)
            {
                if (x < 0 || x >= Width) continue;

                for (var y = _startY; y <= _endY; y++)
                {
                    if (y < 0 || y >= Height) continue;

                    Nodes[x, y].Pathable = World.Instance?.GetTileAt(x, y).InstalledStructure == null;
                }
            }

            sw.Stop();
            //UnityEngine.Debug.Log("Graph update time: " + sw.ElapsedMilliseconds + "ms.");
        }

        private void BuildNodeNeighbours()
        {
            foreach (var node in Nodes)
            {
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
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return null;

            return Nodes[_x, _y];
        }
    }
}

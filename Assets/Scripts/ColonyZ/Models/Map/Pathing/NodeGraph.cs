using System;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map.Pathing
{
    public class NodeGraph
    {
        public static NodeGraph Instance;

        private Action onUpdateGraphCallback;

        public int Width { get; private set; }
        public int Height { get; private set; }

        private Node[,] Nodes { get; set; }

        private NodeGraph()
        {
        }

        /// <summary>
        ///     Creates a new instance of the Node Graph.
        /// </summary>
        public static void Create()
        {
            Instance = new NodeGraph
            {
                Width = World.Instance.Width,
                Height = World.Instance.Height
            };

            Instance.Nodes = new Node[Instance.Width, Instance.Height];

            Instance.BuildFullGraph();

            PathFinder.Create();
        }

        public static void Destroy()
        {
            PathFinder.Destroy();
            Instance = null;
        }

        /// <summary>
        ///     Runs first time build for the graph.
        /// </summary>
        private void BuildFullGraph()
        {
            var nodeID = 0;

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                Nodes[x, y] = new Node(nodeID++,
                    x,
                    y,
                    1.0f,
                    World.Instance.GetTileAt(x, y).GetEnterability() != TileEnterability.None);

            BuildNodeNeighbours();
        }

        /// <summary>
        ///     Update the node graph for a specified area.
        /// </summary>
        /// <param name="_startX"></param>
        /// <param name="_startY"></param>
        /// <param name="_endX"></param>
        /// <param name="_endY"></param>
        public void UpdateGraph(int _startX, int _startY, int _endX, int _endY)
        {
            // Iterate each node and update its Pathable property.
            for (var x = _startX; x <= _endX; x++)
            {
                if (x < 0 || x >= Width) continue;

                for (var y = _startY; y <= _endY; y++)
                {
                    if (y < 0 || y >= Height) continue;

                    var tile = World.Instance.GetTileAt(x, y);
                    Nodes[x, y].Pathable = tile.GetEnterability() != TileEnterability.None;
                }
            }

            onUpdateGraphCallback?.Invoke();
        }

        /// <summary>
        ///     Update a given point for the graph.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public void UpdateGraph(int _x, int _y)
        {
            UpdateGraph(_x, _y, _x, _y);
        }

        private void BuildNodeNeighbours()
        {
            foreach (var node in Nodes) node.ComputeNeighbours();
        }

        /// <summary>
        ///     Safely get a node at a given X and Y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Node GetNodeAt(int _x, int _y)
        {
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return null;

            return Nodes[_x, _y];
        }

        public void RegisterGraphUpdateCallback(Action _callback)
        {
            onUpdateGraphCallback += _callback;
        }
    }
}
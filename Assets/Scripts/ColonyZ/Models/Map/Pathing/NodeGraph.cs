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

        public Node[] Nodes { get; private set; }
        public NodeData[] NodeData { get; private set; }

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
                Height = World.Instance.Height,
            };

            Instance.Nodes = new Node[Instance.Width * Instance.Height];
            Instance.NodeData = new NodeData[Instance.Nodes.Length];

            Instance.BuildFullGraph();

            PathFinder.Create();
        }

        public static void Destroy()
        {
            PathFinder.Destroy();
            Instance = null;
        }

        public bool IsAccessible(int _index)
        {
            return Nodes[_index].Accessible;
        }

        /// <summary>
        ///     Runs first time build for the graph.
        /// </summary>
        private void BuildFullGraph()
        {
            var nodeID = 0;

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                Nodes[x * Width + y] = new Node(nodeID++,
                    x,
                    y,
                    1.0f,
                    World.Instance.GetTileAt(x, y).GetEnterability() != TileEnterability.None);
                NodeData[x * Width + y] = Nodes[x * Width + y].Data;
            }

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
                    var data = new NodeData(x * Width + y, x, y, tile.GetEnterability() != TileEnterability.None);
                    Nodes[x * Width + y].SetData(data);
                    NodeData[x * Width + y] = data;
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

        /// <summary>
        ///     Safely get a node at a given X and Y.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public Node GetNodeAt(int _x, int _y)
        {
            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return new Node(-1, 0, 0, 0, false);

            return Nodes[_x * Width + _y];
        }

        public Node GetNodeAt(int _index)
        {
            return Nodes[_index];
        }

        public void RegisterGraphUpdateCallback(Action _callback)
        {
            onUpdateGraphCallback += _callback;
        }

        private void BuildNodeNeighbours()
        {
            foreach (var node in Nodes)
            {
                foreach (var tile in World.Instance.GetTileAt(node.Data.X, node.Data.Y).DirectNeighbours)
                {
                    node.AddNeighbour(Nodes[World.Instance.GetTileIndex(tile)]);
                }
            }
        }
    }
}
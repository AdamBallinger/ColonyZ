using System.Diagnostics;
using Models.Map;

namespace Models.Pathing
{
    public class NodeGraph
    {
        public static NodeGraph Instance;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ChunkSize { get; private set; }

        //public Node[,] Nodes { get; private set; }

        public NodeChunk[,] Chunks { get; private set; }

        private NodeGraph() { }

        /// <summary>
        /// Creates a new instance of the Node Graph.
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_chunkSize"></param>
        public static void Create(int _width, int _height, int _chunkSize = 8)
        {
            Instance = new NodeGraph
            {
                Width = _width,
                Height = _height,
                ChunkSize = _chunkSize
            };

            //Instance.Nodes = new Node[Instance.Width, Instance.Height];

            Instance.Chunks = new NodeChunk[Instance.Width / Instance.ChunkSize, Instance.Height / Instance.ChunkSize];

            Instance.BuildFullGraph();

            PathFinder.Create();
        }

        /// <summary>
        /// Builds the entire Node Graph.
        /// </summary>
        public void BuildFullGraph()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (var x = 0; x < Width / ChunkSize; x++)
            {
                for (var y = 0; y < Height / ChunkSize; y++)
                {
                    // TODO: Replace 1.0f with an actual movement cost based on tile. For now 1.0 will do.
                    //Nodes[x, y] = new Node(x, y, 1.0f, World.Instance.Tiles[x, y].InstalledStructure == null);

                    Chunks[x, y] = new NodeChunk(x, y, ChunkSize);
                    for (var nodeX = 0; nodeX < ChunkSize; nodeX++)
                    {
                        for (var nodeY = 0; nodeY < ChunkSize; nodeY++)
                        {
                            Chunks[x, y].AddNode(new Node(x * ChunkSize + nodeX, y * ChunkSize + nodeY, 1.0f,
                                World.Instance.Tiles[x * ChunkSize + nodeX, y * ChunkSize + nodeY].InstalledStructure == null), nodeX, nodeY);
                        }
                    }
                }
            }

            BuildChunkNeighbours();
            sw.Stop();
            //UnityEngine.Debug.Log("Graph compute time: " + sw.ElapsedMilliseconds + "ms.");
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
            for (var x = _startX / ChunkSize; x <= _endX / ChunkSize; x++)
            {
                if (x < 0 || x >= Width / ChunkSize) continue;

                for (var y = _startY / ChunkSize; y <= _endY / ChunkSize; y++)
                {
                    if (y < 0 || y >= Height / ChunkSize) continue;

                    // TODO: Replace 1.0f with a real movement cost based on tile. 1.0 will do for now.
                    //Nodes[x, y] = new Node(x, y, 1.0f, World.Instance.Tiles[x, y].InstalledStructure == null);

                    Chunks[x, y] = new NodeChunk(x, y, ChunkSize);
                    for (var nodeX = 0; nodeX < ChunkSize; nodeX++)
                    {
                        for (var nodeY = 0; nodeY < ChunkSize; nodeY++)
                        {
                            Chunks[x, y].AddNode(new Node(x * ChunkSize + nodeX, y * ChunkSize + nodeY, 1.0f,
                                World.Instance.Tiles[x * ChunkSize + nodeX, y * ChunkSize + nodeY].InstalledStructure == null), nodeX, nodeY);
                        }
                    }
                }
            }

            BuildChunkNeighbours();

            // Recompute neighbour nodes for the rebuilt nodes, and with some padding to surrounding tiles to allow them to update their own 
            // list of neighbours.
            //for (var x = _startX - _padding; x < _endX + _padding; x++)
            //{
            //    if (x < 0 || x >= Width) continue;

            //    for (var y = _startY - _padding; y < _endY + _padding; y++)
            //    {
            //        if (y < 0 || y >= Height) continue;

            //        // Optimisation to ignore neigbour generation for none pathable nodes since they never get
            //        // evaluated by the path finder anyway.
            //        if (!Nodes[x, y].Pathable) continue;

            //        Nodes[x, y].ComputeNeighbours();
            //    }
            //}
        }

        private void BuildNodeNeighbours()
        {
            //foreach (var node in Nodes)
            //{
            //    // Ignore none pathable nodes since they wont be evaulated by the path finder.
            //    if (!node.Pathable) continue;

            //    node.ComputeNeighbours();
            //}
        }

        private void BuildChunkNeighbours()
        {
            foreach (var chunk in Chunks)
            {
                foreach (var node in chunk.Nodes)
                {
                    node.ComputeNeighbours();
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
            //if (_x < 0 || _x >= Width || _y < 0 || _y >= Height)
            //{
            //    return null;
            //}

            //return Nodes[_x, _y];

            if (_x < 0 || _x >= Width || _y < 0 || _y >= Height) return null;

            var cX = _x / ChunkSize;
            var cY = _y / ChunkSize;

            if (cX < 0 || cX >= Width / ChunkSize || cY < 0 || cY >= Height / ChunkSize) return null;

            return Chunks[cX, cY].Nodes[_x % ChunkSize, _y % ChunkSize];
        }

        /// <summary>
        /// Gets the Node Chunk at a given X and Y point in the world.
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        public NodeChunk GetChunkInWorld(int _x, int _y)
        {
            var cX = _x / ChunkSize;
            var cY = _y / ChunkSize;

            if (cX < 0 || cX >= Width / ChunkSize || cY < 0 || cY >= Height / ChunkSize) return null;

            return Chunks[cX, cY];
        }

        public NodeChunk GetChunkInWorld(Node _node)
        {
            return _node == null ? null : GetChunkInWorld(_node.X, _node.Y);
        }

        public NodeChunk GetChunkAt(int _x, int _y)
        {
            if (_x < 0 || _x >= Width / ChunkSize || _y < 0 || _y >= Height / ChunkSize) return null;

            return Chunks[_x, _y];
        }
    }
}

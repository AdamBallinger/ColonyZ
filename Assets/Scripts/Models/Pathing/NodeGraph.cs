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

            Instance.Chunks = new NodeChunk[Instance.Width / Instance.ChunkSize, Instance.Height / Instance.ChunkSize];

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

            for (var x = 0; x < Width / ChunkSize; x++)
            {
                for (var y = 0; y < Height / ChunkSize; y++)
                {
                    Chunks[x, y] = new NodeChunk(x, y, ChunkSize);

                    // Create the nodes for the chunk at x,y
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

            BuildNodeNeighbours();
            sw.Stop();
            //UnityEngine.Debug.Log("Graph build time: " + sw.ElapsedMilliseconds + "ms.");
        }

        /// <summary>
        /// Update the node graph around a specified are with an optional padding. By default, padding is 1 chunk padding
        /// </summary>
        /// <param name="_startX"></param>
        /// <param name="_startY"></param>
        /// <param name="_endX"></param>
        /// <param name="_endY"></param>
        /// <param name="_padding"></param>
        public void UpdateGraph(int _startX, int _startY, int _endX, int _endY, int _padding = 1)
        {
            var sw = new Stopwatch();
            sw.Start();

            // Iterate each chunk
            for (var x = _startX / ChunkSize; x <= _endX / ChunkSize; x++)
            {
                if (x < 0 || x >= Width / ChunkSize) continue;

                for (var y = _startY / ChunkSize; y <= _endY / ChunkSize; y++)
                {
                    if (y < 0 || y >= Height / ChunkSize) continue;

                    // Iterate every node in the chunk at x,y and update it.
                    for (var nodeX = 0; nodeX < ChunkSize; nodeX++)
                    {
                        for (var nodeY = 0; nodeY < ChunkSize; nodeY++)
                        {
                            Chunks[x, y].UpdateNode(nodeX, nodeY, 
                                World.Instance.Tiles[x * ChunkSize + nodeX, y * ChunkSize + nodeY].InstalledStructure == null);
                        }
                    }
                }
            }

            sw.Stop();
            //UnityEngine.Debug.Log("Graph update time: " + sw.ElapsedMilliseconds + "ms.");
        }

        private void BuildNodeNeighbours()
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

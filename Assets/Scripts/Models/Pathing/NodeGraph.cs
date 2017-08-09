using Models.Map;

namespace Models.Pathing
{
	public class NodeGraph 
	{
        
        public int Width { get; }
        public int Height { get; }

        public Node[,] Nodes { get; }

        public NodeGraph(int _width, int _height)
        {
            Width = _width;
            Height = _height;

            Nodes = new Node[Width, Height];
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
            for(var x = _x - _size / 2; x < _x + _size / 2; x++)
            {
                if (x < 0 || x > Width) continue;

                for(var y = _y - _size / 2; y < _y + _size / 2; y++)
                {
                    if(y < 0 || y > Height) continue;

                    Nodes[x, y] = new Node(x, y, World.Instance.Tiles[x, y].InstalledStructure == null);
                }
            }
        }
	}
}

using Models.Map.Tiles;

namespace Models.Map.Regions
{
    public enum EdgeSpanDirection
    {
        Right,
        Up
    }

    public struct EdgeSpan
    {
        private Tile Root { get; }

        public EdgeSpanDirection Direction { get; }

        public int Size { get; }

        public EdgeSpan(Tile _root, EdgeSpanDirection _direction, int _size)
        {
            Root = _root;
            Direction = _direction;
            Size = _size;
        }

        public ulong UniqueHashCode()
        {
            var code = (ulong) Root.GetHashCode();
            code += Direction == EdgeSpanDirection.Right
                ? 72324523523UL
                : 249249877687462UL
                  * (ulong) Size;
            return code;
        }
    }
}
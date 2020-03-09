using UnityEngine;

namespace Models.Map.Areas
{
    public abstract class Area
    {
        public string AreaName { get; protected set; }
        
        public bool RequiresRoom { get; protected set; }
        
        public Vector2 MinimumSize { get; protected set; }
        
        public Vector2 Origin { get; }
        
        public Vector2 Size { get; }
        
        protected Area(int _x, int _y, int _width, int _height)
        {
            MinimumSize = Vector2.one;
            Origin = new Vector2(_x, _y);
            Size = new Vector2(_width, _height);
        }
        
        public bool IsValidSize()
        {
            return Size.x >= MinimumSize.x && Size.y >= MinimumSize.y;
        }
    }
}
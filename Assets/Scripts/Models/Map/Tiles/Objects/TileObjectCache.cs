using System.Collections.Generic;

namespace Models.Map.Tiles.Objects
{
    public static class TileObjectCache
    {
        public static List<TileObject> TileObjects { get; } = new List<TileObject>();
        
        public static void Add(TileObject _object)
        {
            if (_object == null)
            {
                return;
            }
            
            if(TileObjects.Contains(_object))
            {
                return;
            }
            
            TileObjects.Add(_object);
        }
    }
}
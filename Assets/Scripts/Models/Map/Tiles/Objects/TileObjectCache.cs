using System.Collections.Generic;

namespace Models.Map.Tiles.Objects
{
    public static class TileObjectCache
    {
        public static List<TileObject> TileObjects { get; } = new List<TileObject>();
        
        public static TileObject FoundationObject { get; private set; }
        
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
            
            // Don't add foundation to the cache, but instead the global reference.
            if (_object is FoundationObject)
            {
                FoundationObject = _object;
                return;
            }
            
            TileObjects.Add(_object);
        }
    }
}
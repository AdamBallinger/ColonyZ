using System.Collections.Generic;
using Models.Map.Tiles.Objects;

namespace Models.Map.Tiles
{
    public static class TileObjectRegistry
    {
        private static Dictionary<string, TileObject> objectRegistry = new Dictionary<string, TileObject>();

        public static void RegisterTileObject(TileObject _instance)
        {
            if(objectRegistry.ContainsKey(_instance.ObjectName))
            {
                return;
            }

            objectRegistry.Add(_instance.ObjectName, _instance);
        }

        public static TileObject GetObject(string _objectName)
        {
            return !objectRegistry.ContainsKey(_objectName) ? null : objectRegistry[_objectName].Clone();
        }
    }
}

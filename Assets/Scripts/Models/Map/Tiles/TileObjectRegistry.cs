using System.Collections.Generic;
using Models.Map.Tiles.Objects;

namespace Models.Map.Tiles
{
    public static class TileObjectRegistry
    {
        private static Dictionary<string, TileObject> structureRegistry = new Dictionary<string, TileObject>();

        public static void RegisterTileStructure(TileObject _instance)
        {
            if(structureRegistry.ContainsKey(_instance.StructureName))
            {
                return;
            }

            structureRegistry.Add(_instance.StructureName, _instance);
        }

        public static TileObject GetStructure(string _structureName)
        {
            return !structureRegistry.ContainsKey(_structureName) ? null : structureRegistry[_structureName].Clone();
        }
    }
}

using System.Collections.Generic;
using Models.Map.Structures;

namespace Models.Map
{
    public class TileStructureRegistry
    {
        private static Dictionary<string, TileStructure> structureRegistry = new Dictionary<string, TileStructure>();

        public static void RegisterTileStructure(TileStructure _instance)
        {
            if(structureRegistry.ContainsKey(_instance.StructureName))
            {
                return;
            }

            structureRegistry.Add(_instance.StructureName, _instance);
        }

        public static TileStructure GetStructure(string _structureName)
        {
            return !structureRegistry.ContainsKey(_structureName) ? null : structureRegistry[_structureName].Clone();
        }
    }
}

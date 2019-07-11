using System.Collections.Generic;
using UnityEngine;

namespace Models.Map.Tiles
{
    public static class TileManager
    {
        private static readonly string dataRoot = "TileDefs/";

        private static Dictionary<string, TileDefinition> tileDefinitions = new Dictionary<string, TileDefinition>();

        public static TileDefinition GetTileDefinition(string _name)
        {
            if (!tileDefinitions.ContainsKey(_name))
            {
                Debug.LogError($"No tile definition for name: {_name}");
                return null;
            }

            return tileDefinitions[_name];
        }

        public static void LoadDefinitions()
        {
            tileDefinitions.Clear();

            var definitions = Resources.LoadAll<TileDefinition>(dataRoot);

            foreach (var def in definitions)
            {
                if (tileDefinitions.ContainsKey(def.TileName))
                {
                    Debug.LogWarning($"Skipping tile definition: {def.TileName} as it was already loaded.");
                    continue;
                }
                
                tileDefinitions.Add(def.TileName, def);
            }
        }
    }
}
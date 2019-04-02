using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map.Tiles
{
    public static class TileManager
    {
        private static readonly string dataRoot = "Game_Data/Defs/Tiles/";

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

            var data = Resources.LoadAll<TextAsset>(dataRoot);

            foreach (var asset in data)
            {
                var def = JsonConvert.DeserializeObject<TileDefinition>(asset.text);

                if (tileDefinitions.ContainsKey(def.Name))
                {
                    Debug.LogWarning($"Skipping tile def: {def.Name} as it was already loaded.");
                    continue;
                }
                
                tileDefinitions.Add(def.Name, def);
            }
        }
    }
}
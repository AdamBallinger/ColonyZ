using System;
using System.IO;
using System.Text;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ColonyZ.Models.Saving
{
    public class SaveGameHandler
    {
        private static string Save_Game_Root { get; }
            = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\ColonyZTest";

        private static string WorldFile { get; } = Save_Game_Root + "\\world.json";
        private static string ObjectsFile { get; } = Save_Game_Root + "\\objects.json";
        private static string EntitiesFile { get; } = Save_Game_Root + "\\entities.json";

        public SaveGameHandler()
        {
            ValidateSaveGameDirectory();
        }

        private void ValidateSaveGameDirectory()
        {
            if (!Directory.Exists(Save_Game_Root))
            {
                Debug.Log($"Creating save game directory: {Save_Game_Root}");
                Directory.CreateDirectory(Save_Game_Root);
            }
        }

        public void SaveAll()
        {
            SaveWorld();
            SaveObjects();
            SaveEntities();
        }

        public void LoadAll()
        {
            LoadWorld();
            LoadObjects();
            LoadEntities();
        }

        private void SaveWorld()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            World.Instance.OnSave(writer);
            writer.WriteEndObject();

            WriteJsonToFile(sb.ToString(), WorldFile);
        }

        private void SaveObjects()
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.Indented;

            writer.WriteStartArray();
            foreach (var obj in World.Instance.Objects)
            {
                // Don't need to write tree edges to file.
                if (obj.OriginTile.IsMapEdge) continue;
                writer.WriteStartObject();
                obj.OnSave(writer);
                writer.WriteEndObject();
            }

            writer.WriteEnd();
            WriteJsonToFile(sb.ToString(), ObjectsFile);
        }

        private void SaveEntities()
        {
        }

        private void WriteJsonToFile(string _json, string _file)
        {
            File.WriteAllText(_file, _json);
        }

        private void LoadWorld()
        {
        }

        private void LoadObjects()
        {
            var objectsJson = JArray.Parse(File.ReadAllText(ObjectsFile));

            for (var i = 0; i < objectsJson.Count; i++)
            {
                //var x = objectsJson[i]["tile_x"].Value<int>();
                //var y = objectsJson[i]["tile_y"].Value<int>();
                var obj = TileObjectCache.GetObject(objectsJson[i]["id"].ToString());
                //World.Instance.GetTileAt(x, y).SetObject(obj, false);
                obj.OnLoad(objectsJson[i]);
            }
        }

        private void LoadEntities()
        {
        }

        public static bool SaveGamePresent()
        {
            return File.Exists(WorldFile);
        }
    }
}
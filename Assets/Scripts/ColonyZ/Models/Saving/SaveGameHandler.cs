using System;
using System.IO;
using System.Text;
using ColonyZ.Models.Map;
using Newtonsoft.Json;
using UnityEngine;

namespace ColonyZ.Models.Saving
{
    public class SaveGameHandler
    {
        private string Save_Game_Root { get; }
        private string WorldFile { get; }
        private string ObjectsFile { get; }
        private string EntitiesFile { get; }

        public SaveGameHandler()
        {
            Save_Game_Root = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\ColonyZTest";
            WorldFile = Save_Game_Root + "\\world.json";
            ObjectsFile = Save_Game_Root + "\\objects.json";
            EntitiesFile = Save_Game_Root + "\\entities.json";
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

            foreach (var obj in World.Instance.Objects)
            {
                // Don't need to write tree edges to file.
                if (obj.OriginTile.IsMapEdge) continue;
                writer.WriteStartObject();
                obj.OnSave(writer);
                writer.WriteEndObject();
            }

            WriteJsonToFile(sb.ToString(), ObjectsFile);
        }

        private void SaveEntities()
        {
        }

        private void WriteJsonToFile(string _json, string _file)
        {
            File.WriteAllText(_file, _json);
        }

        public void Load()
        {
        }
    }
}
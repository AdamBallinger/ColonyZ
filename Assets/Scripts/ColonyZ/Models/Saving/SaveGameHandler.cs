using System;
using System.IO;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles.Objects;
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

        private SaveGameWriter saveWriter;

        public SaveGameHandler()
        {
            saveWriter = new SaveGameWriter();
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
            saveWriter.StartNew();
            SaveWorld();
            saveWriter.StartNew();
            SaveObjects();
            saveWriter.StartNew();
            SaveEntities();
        }

        public void LoadWorldData(WorldDataProvider _provider)
        {
            var worldJson = JObject.Parse(File.ReadAllText(WorldFile));
            _provider.OnLoad(worldJson);
        }

        public void LoadAll()
        {
            if (World.Instance == null)
            {
                Debug.LogError("Can't call SaveGameHandler::LoadAll until world instance has been created.");
                return;
            }

            LoadObjects();
            LoadEntities();
        }

        private void SaveWorld()
        {
            saveWriter.BeginObject();
            World.Instance.WorldProvider.OnSave(saveWriter);
            saveWriter.EndObject();

            WriteJsonToFile(saveWriter.GetJson(), WorldFile);
        }

        private void SaveObjects()
        {
            saveWriter.BeginArray();

            foreach (var obj in World.Instance.Objects)
            {
                // Don't need to write tree edges to file, or objects that no longer need saving.
                if (obj.OriginTile.IsMapEdge || !obj.CanSave()) continue;
                saveWriter.BeginObject();
                obj.OnSave(saveWriter);
                saveWriter.EndObject();
            }

            saveWriter.EndArray();

            WriteJsonToFile(saveWriter.GetJson(), ObjectsFile);
        }

        private void SaveEntities()
        {
            saveWriter.BeginObject();
            SaveItems();
            SaveLiving();
            saveWriter.EndObject();

            WriteJsonToFile(saveWriter.GetJson(), EntitiesFile);
        }

        private void SaveItems()
        {
            saveWriter.WriteProperty("Items");
            saveWriter.BeginArray();
            foreach (var item in World.Instance.Items)
            {
                saveWriter.BeginObject();
                item.OnSave(saveWriter);
                saveWriter.EndObject();
            }

            saveWriter.EndArray();
        }

        private void SaveLiving()
        {
            saveWriter.WriteProperty("Living");
            saveWriter.BeginArray();
            foreach (var living in World.Instance.Characters)
            {
                saveWriter.BeginObject();
                living.OnSave(saveWriter);
                saveWriter.EndObject();
            }

            saveWriter.EndArray();
        }

        private void WriteJsonToFile(string _json, string _file)
        {
            File.WriteAllText(_file, _json);
        }

        private void LoadObjects()
        {
            var objectsJson = JArray.Parse(File.ReadAllText(ObjectsFile));

            for (var i = 0; i < objectsJson.Count; i++)
            {
                var obj = TileObjectCache.GetObject(objectsJson[i]["id"].ToString());
                obj.OnLoad(objectsJson[i]);
            }
        }

        private void LoadEntities()
        {
            var entityJson = JObject.Parse(File.ReadAllText(EntitiesFile));
            foreach (var itemData in entityJson["Items"])
            {
                var tempEntity = new ItemEntity();
                tempEntity.OnLoad(itemData);
            }

            foreach (var livingData in entityJson["Living"])
            {
                var tempEntity = new LivingEntity();
                tempEntity.OnLoad(livingData);
            }
        }

        public static bool SaveGamePresent()
        {
            return File.Exists(WorldFile);
        }
    }
}
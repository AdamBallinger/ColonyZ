using System;
using System.IO;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Utils;
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
        private static string ZonesFile { get; } = Save_Game_Root + "\\zones.json";
        private static string JobsFile { get; } = Save_Game_Root + "\\jobs.json";

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
            saveWriter.StartNew();
            SaveZones();
            saveWriter.StartNew();
            SaveJobs();
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
            LoadZones();
            LoadJobs();
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

        private void SaveZones()
        {
            saveWriter.BeginObject();
            saveWriter.WriteProperty("Zones");
            saveWriter.BeginArray();
            foreach (var zone in ZoneManager.Instance.Zones)
            {
                saveWriter.BeginObject();
                zone.OnSave(saveWriter);
                saveWriter.EndObject();
            }

            saveWriter.EndArray();
            saveWriter.EndObject();

            WriteJsonToFile(saveWriter.GetJson(), ZonesFile);
        }

        private void SaveJobs()
        {
            saveWriter.BeginObject();
            saveWriter.WriteProperty("Jobs");
            saveWriter.BeginArray();
            foreach (var job in JobManager.Instance.Jobs)
            {
                saveWriter.BeginObject();
                job.OnSave(saveWriter);
                saveWriter.EndObject();
            }

            saveWriter.EndArray();
            saveWriter.EndObject();

            WriteJsonToFile(saveWriter.GetJson(), JobsFile);
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
                var data = TileObjectDataCache.GetData(objectsJson[i]["data_name"].ToString());
                // Foundations don't have a factory.
                var obj = data.ObjectName == "Foundation"
                    ? new FoundationObject(data)
                    : ObjectFactoryUtil.GetFactory(data.FactoryType).GetObject(data);
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

        private void LoadZones()
        {
            var zonesJson = JObject.Parse(File.ReadAllText(ZonesFile));
            foreach (var zoneData in zonesJson["Zones"])
            {
                var zoneType = Type.GetType(zoneData["zone_type"].Value<string>());
                if (zoneType == null)
                {
                    Debug.LogError("Attempted to load a null zone type. Skipping...");
                    continue;
                }

                var tempZone = Activator.CreateInstance(zoneType) as Zone;
                tempZone?.OnLoad(zoneData);
            }
        }

        private void LoadJobs()
        {
            var jobsJson = JObject.Parse(File.ReadAllText(JobsFile));
            foreach (var jobData in jobsJson["Jobs"])
            {
                var jobType = Type.GetType(jobData["job_type"].Value<string>());
                if (jobType == null)
                {
                    Debug.LogError("Attempted to load a null job type. Skipping...");
                    continue;
                }

                var jobTarget = World.Instance.GetTileAt(jobData["target"].Value<int>());
                var job = (Job) Activator.CreateInstance(jobType, jobTarget);
                job.OnLoad(jobData);
                JobManager.Instance.AddJob(job);
            }
        }

        public static bool SaveGamePresent()
        {
            return File.Exists(WorldFile);
        }
    }
}
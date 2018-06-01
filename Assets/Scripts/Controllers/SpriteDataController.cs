using System;
using System.Collections.Generic;
using Controllers.Tiles;
using Models.Sprites;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Controllers
{
    public class SpriteDataController
    {
        //    private static Dictionary<Type, List<ISpriteData>> dataDict = new Dictionary<Type, List<ISpriteData>>();

        //    private static readonly string dataRoot = "Game_Data/Sprite_Data/";

        //       public static void RegisterSpriteDataType<T>() where T : ISpriteData
        //       {
        //           if(!dataDict.ContainsKey(typeof(T)))
        //               dataDict.Add(typeof(T), new List<ISpriteData>());
        //           else
        //               Debug.LogError("[SpriteDataController.RegisterSpriteDataType] -> " +
        //                                          $"Attempted to add duplicate type key: {typeof(T).Name} to sprite data dict!");
        //       }

        //       /// <summary>
        //       /// Loads the sprite data for the given object name E.g. "Wood_Wall". This must exist as a .json file in the
        //       /// sprite_data resources folder in the respective sprite data type folder.
        //       /// </summary>
        //       /// <typeparam name="T"></typeparam>
        //       /// <param name="_dataName"></param>
        //       public static void Load<T>(string _dataName) where T : ISpriteData
        //       {
        //           if(!dataDict.ContainsKey(typeof(T)))
        //           {
        //               Debug.LogError("[SpriteDataController.Load] -> " +
        //                                          $"Failed to load sprite data for: {typeof(T).Name} as it doesn't exist in the dict!");
        //           }
        //           else
        //           {
        //               var path = $"{dataRoot}{typeof(T).Name}/{_dataName}";

        //               var dataJsonFile = Resources.Load<TextAsset>(path);
        //               var dataObj = JsonConvert.DeserializeObject<T>(dataJsonFile.text);

        //               dataDict[typeof(T)].Add(dataObj);
        //               SpriteController.LoadSpriteData(dataObj);
        //           }
        //       }

        //       /// <summary>
        //       /// Gets a SpriteData class for the given object name. E.g. GetSpriteDataFor("Wood_Wall")
        //       /// </summary>
        //       /// <typeparam name="T"></typeparam>
        //       /// <param name="_objectName"></param>
        //       /// <returns></returns>
        //	public static T GetSpriteDataFor<T>(string _objectName) where T : ISpriteData
        //	{
        //	    foreach(var keyPair in dataDict)
        //	    {
        //	        if(keyPair.Key == typeof(T))
        //	        {
        //	            foreach(var data in keyPair.Value)
        //	            {
        //	                if (data.GetMappedObjectName() == _objectName)
        //	                {
        //	                    return (T) data;
        //	                }
        //	            }
        //	        }
        //	    }

        //	    return default(T);
        //	}
        private static readonly string dataRoot = "Game_Data/Sprite_Data/";

        private static Dictionary<string, SpriteData> dataMap = new Dictionary<string, SpriteData>();

        /// <summary>
        /// Loads all sprite data files and adds the sprites to the sprite cache.
        /// </summary>
        public static void LoadSpriteData()
        {
            var data = Resources.LoadAll(dataRoot, typeof(TextAsset));

            foreach(var obj in data)
            {
                var asset = (TextAsset) obj;
                var spriteData = JsonConvert.DeserializeObject<SpriteData>(asset.text);

                dataMap.Add(asset.name, spriteData);

                if(spriteData.SpriteType == SpriteType.Single)
                {
                    LoadSprite(spriteData);
                }
                else
                {
                    LoadTileSet(spriteData);
                }
            }
        }

        public static SpriteData GetSpriteData(string _dataName)
        {
            return dataMap.ContainsKey(_dataName) ? dataMap[_dataName] : null;
        }

        /// <summary>
        /// Loads the sprite at the given path to the sprite cache.
        /// </summary>
        /// <param name="_spriteData"></param>
        private static void LoadSprite(SpriteData _spriteData)
        {
            var sprite = Resources.Load<Sprite>(_spriteData.ResourcePath);

            if (sprite == null)
            {
                Debug.LogError($"[SpriteDataController.LoadSprite] Failed to load sprite at path: {_spriteData.ResourcePath}");
                return;
            }

            SpriteCache.AddSprite(_spriteData.SpriteGroup, sprite);
        }

        /// <summary>
        /// Loads the tileset at the given path to the sprite cache.
        /// </summary>
        /// <param name="_spriteData"></param>
        private static void LoadTileSet(SpriteData _spriteData)
        {
            var tileset = Resources.LoadAll<Sprite>(_spriteData.ResourcePath);

            if (tileset == null)
            {
                Debug.LogError($"[SpriteDataController.LoadTileSet] Failed to load tileset at path: {_spriteData.ResourcePath}");
                return;
            }

            foreach (var sprite in tileset)
            {
                SpriteCache.AddSprite(_spriteData.SpriteGroup, sprite);
            }
        }
    }
}

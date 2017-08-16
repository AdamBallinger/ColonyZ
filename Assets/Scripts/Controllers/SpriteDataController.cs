using System;
using System.Collections.Generic;
using System.IO;
using Controllers.Tiles;
using Models.Map;
using Models.Sprites;
using Newtonsoft.Json;
using UnityEngine;

namespace Controllers
{
	public class SpriteDataController
	{

	    private static Dictionary<Type, List<ISpriteData>> dataDict = new Dictionary<Type, List<ISpriteData>>();

	    private static readonly string dataRoot = "Game_Data/Sprite_Data/";

        public static void RegisterSpriteDataType<T>() where T : ISpriteData
        {
            if(!dataDict.ContainsKey(typeof(T)))
                dataDict.Add(typeof(T), new List<ISpriteData>());
            else
                Debug.LogError("[SpriteDataController.RegisterSpriteDataType] -> " +
                                           $"Attempted to add duplicate type key: {typeof(T).Name} to sprite data dict!");
        }

        /// <summary>
        /// Loads the sprite data for the given SpriteData object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_dataName"></param>
        public static void LoadSpriteData<T>(string _dataName) where T : ISpriteData
        {
            if(!dataDict.ContainsKey(typeof(T)))
            {
                Debug.LogError("[SpriteDataController.LoadSpriteData] -> " +
                                           $"Failed to load sprite data for: {typeof(T).Name} as it doesn't exist in the dict!");
            }
            else
            {
                var path = $"{dataRoot}{typeof(T).Name}/{_dataName}";

                var dataJsonFile = Resources.Load<TextAsset>(path);
                var dataObj = JsonConvert.DeserializeObject<T>(dataJsonFile.text);

                dataDict[typeof(T)].Add(dataObj);
                SpriteController.LoadSpriteData(dataObj);
            }
        }

        /// <summary>
        /// Gets a SpriteData class for the given object name. E.g. GetSpriteDataFor("Wood_Wall")
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_objectName"></param>
        /// <returns></returns>
		public static T GetSpriteDataFor<T>(string _objectName) where T : ISpriteData
		{
		    foreach(var keyPair in dataDict)
		    {
		        if(keyPair.Key == typeof(T))
		        {
		            foreach(var data in keyPair.Value)
		            {
		                if (data.GetMappedObjectName() == _objectName)
		                {
		                    return (T) data;
		                }
		            }
		        }
		    }

		    return default(T);
		}

	}
}

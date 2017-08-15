using System;
using System.Collections.Generic;
using Controllers.Tiles;
using Models.Sprites;

namespace Controllers
{
	public class SpriteDataController
	{

	    private static Dictionary<Type, List<ISpriteData>> dataDict = new Dictionary<Type, List<ISpriteData>>();

        public static void RegisterSpriteDataType<T>() where T : ISpriteData
        {
            if(!dataDict.ContainsKey(typeof(T)))
                dataDict.Add(typeof(T), new List<ISpriteData>());
            else
                UnityEngine.Debug.LogError("[SpriteDataController.RegisterSpriteDataType] -> " +
                                           $"Attempted to add duplicate type key: {typeof(T).Name} to sprite data dict!");
        }

        /// <summary>
        /// Loads the sprite data for the given SpriteData object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_t"></param>
        public static void LoadSpriteData<T>(T _t) where T : ISpriteData
        {
            if(!dataDict.ContainsKey(_t.GetType()))
            {
                UnityEngine.Debug.LogError("[SpriteDataController.LoadSpriteData] -> " +
                                           $"Failed to load sprite data for: {_t.GetType().Name} as it doesn't exist in the dict!");
            }
            else
            {
                dataDict[_t.GetType()].Add(_t);
                SpriteController.LoadSpriteData(_t);
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

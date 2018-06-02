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

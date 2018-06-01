using System.Collections.Generic;
using Models.Sprites;
using UnityEngine;

namespace Controllers
{
    public class SpriteCache
    {
        private static Dictionary<string, List<Sprite>> spriteCache = new Dictionary<string, List<Sprite>>();

        public static Sprite GetSprite(string _spriteName)
        {
            //return spriteCache.ContainsKey(_spriteName) ? spriteCache[_spriteName] : null;
            return null;
        }

        public static Sprite GetSprite(string _spriteGroup, string _spriteName)
        {
            return GetSprite(_spriteName);
        }

        public static Sprite GetSprite(SpriteData _data)
        {
            return null;
        }

        public static void AddSprite(string _spriteGroup, string _spriteName, Sprite _sprite)
        {
            if(spriteCache.ContainsKey(_spriteName))
            {
                // Already exists in cache.
                return;
            }

            //spriteCache.Add(_spriteName, _sprite);
        }
    }
}

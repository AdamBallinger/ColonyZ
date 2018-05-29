using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class SpriteCache
    {
        private static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        public static Sprite GetSprite(string _spriteName)
        {
            return spriteCache.ContainsKey(_spriteName) ? spriteCache[_spriteName] : null;
        }

        public static void AddSprite(string _spriteName, Sprite _sprite)
        {
            if(spriteCache.ContainsKey(_spriteName))
            {
                // Already exists in cache.
                return;
            }

            spriteCache.Add(_spriteName, _sprite);
        }
    }
}

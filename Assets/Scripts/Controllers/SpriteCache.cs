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
            return GetSprite("default", _spriteName);
        }

        public static Sprite GetSprite(string _spriteGroup, string _spriteName)
        {
            if(spriteCache.ContainsKey(_spriteGroup))
            {
                foreach (var sprite in spriteCache[_spriteGroup])
                {
                    if (sprite.name.Equals(_spriteName))
                    {
                        return sprite;
                    }
                }
            }

            return null;
        }

        public static Sprite GetSprite(string _spriteGroup, int _groupIndex)
        {
            if(spriteCache.ContainsKey(_spriteGroup))
            {
                if(spriteCache[_spriteGroup].Count <= _groupIndex)
                {
                    return spriteCache[_spriteGroup][_groupIndex];
                }
            }

            return null;
        }

        public static Sprite GetSprite(SpriteData _data)
        {
            return null;
        }

        public static void AddSprite(string _spriteGroup, Sprite _sprite)
        {
            if(_sprite == null)
            {
                // Don't add null sprites to the cache
                return;
            }

            if(!spriteCache.ContainsKey(_spriteGroup))
            {
                spriteCache.Add(_spriteGroup, new List<Sprite>());
            }

            if(spriteCache[_spriteGroup].Contains(_sprite))
            {
                // Sprite already exists in the cache for this group
                return;
            }

            spriteCache[_spriteGroup].Add(_sprite);
        }
    }
}

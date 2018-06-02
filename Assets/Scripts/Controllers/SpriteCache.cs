using System.Collections.Generic;
using Models.Map;
using Models.Map.Structures;
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
                if(spriteCache[_spriteGroup].Count >= _groupIndex - 1)
                {
                    return spriteCache[_spriteGroup][_groupIndex];
                }
            }

            return null;
        }

        public static Sprite GetSprite(TileStructure _structure)
        {
            if(_structure == null)
            {
                return null;
            }

            if(spriteCache.ContainsKey(_structure.SpriteData.SpriteGroup))
            {
                var spriteData = _structure.SpriteData;

                if(spriteData.SpriteType == SpriteType.Single)
                {
                    return spriteCache[spriteData.SpriteGroup][0];
                }

                var index = _structure.Type == TileStructureType.Single_Tile ? _structure.GetSpriteIndex() : 
                    TileBitMask.ComputeBitmaskValue(_structure.Tile, BitmaskEvaluationType.Tile_Structure);
                return spriteCache[spriteData.SpriteGroup][index];
            }

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

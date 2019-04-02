using System.Collections.Generic;
using Models.Map;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Models.Sprites;
using UnityEngine;

namespace Controllers
{
    public static class SpriteCache
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

        public static Sprite GetSprite(TileObject _object)
        {
            if(_object == null)
            {
                return null;
            }

            if(spriteCache.ContainsKey(_object.SpriteData.SpriteGroup))
            {
                var spriteData = _object.SpriteData;

                if(spriteData.SpriteType == SpriteType.Single)
                {
                    return spriteCache[spriteData.SpriteGroup][0];
                }

                var index = _object.Type == TileObjectType.Single_Tile ? _object.GetSpriteIndex() : 
                    TileBitMask.ComputeBitmaskValue(_object.Tile, BitmaskEvaluationType.Tile_Structure);
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

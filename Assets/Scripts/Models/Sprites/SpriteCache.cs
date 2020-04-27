﻿using System.Collections.Generic;
using Models.Items;
using Models.Map.Tiles.Objects;
using UnityEngine;

namespace Models.Sprites
{
    public static class SpriteCache
    {
        private static Dictionary<string, List<Sprite>> spriteCache = new Dictionary<string, List<Sprite>>();

        public static Sprite GetSprite(string _spriteGroup, int _groupIndex)
        {
            if (spriteCache.ContainsKey(_spriteGroup))
            {
                if (spriteCache[_spriteGroup].Count >= _groupIndex - 1)
                {
                    return spriteCache[_spriteGroup][_groupIndex];
                }
            }

            return null;
        }

        public static Sprite GetSprite(TileObject _object)
        {
            if (spriteCache.ContainsKey(_object.SpriteData.SpriteGroup))
            {
                var spriteData = _object.SpriteData;

                if (spriteData.SpriteCount == 1)
                {
                    return spriteCache[spriteData.SpriteGroup][0];
                }

                var index = !_object.DynamicSprite
                    ? _object.GetSpriteIndex()
                    : SpriteBitMask.GetObjectWorldIndex(_object.Tile);
                return spriteCache[spriteData.SpriteGroup][index];
            }

            return null;
        }

        public static Sprite GetSprite(Item _item)
        {
            if (spriteCache.ContainsKey(_item.ItemSpriteData.SpriteGroup))
            {
                var spriteData = _item.ItemSpriteData;

                return spriteCache[spriteData.SpriteGroup][_item.SpriteIndex];
            }

            return null;
        }

        public static void AddSprite(string _spriteGroup, Sprite _sprite)
        {
            if (_sprite == null)
            {
                return;
            }

            if (!spriteCache.ContainsKey(_spriteGroup))
            {
                spriteCache.Add(_spriteGroup, new List<Sprite>());
            }

            if (spriteCache[_spriteGroup].Contains(_sprite))
            {
                return;
            }

            spriteCache[_spriteGroup].Add(_sprite);
        }

        public static void AddSprites(string _spriteGroup, IEnumerable<Sprite> _sprites)
        {
            foreach (var sprite in _sprites)
            {
                AddSprite(_spriteGroup, sprite);
            }
        }
    }
}
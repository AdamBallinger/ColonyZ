using System.Collections.Generic;
using Models.World;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Abstract Tile sprite controller class for setting tiles sprites based on types.
    /// </summary>
	public abstract class TileSpriteController : MonoBehaviour
    {
        protected Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        /// <summary>
        /// Get the sprite for a specific tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
	    public abstract Sprite GetSprite(Tile _tile);

        /// <summary>
        /// Gets a given sprite name from the sprite cache for this sprite controller. If the sprite isn't present in the cache, then
        /// the sprite will be loaded from resources if it exists, and placed inside the cache.
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
        protected Sprite GetSpriteFromCache(TileSpriteData _data)
        {
            if (spriteCache.ContainsKey(_data.SpriteName))
            {
                return spriteCache[_data.SpriteName];
            }

            if (_data.IsSpriteInTileset)
            {
                var tileset = Resources.LoadAll<Sprite>(_data.SpriteResourceLocation);
                foreach (var sprite in tileset)
                {
                    if (sprite.name == _data.SpriteName)
                    {
                        AddSpriteToCache(sprite, _data.SpriteName);
                        return sprite;
                    }
                }
            }
            else
            {
                var sprite = Resources.Load<Sprite>(_data.SpriteResourceLocation);
                AddSpriteToCache(sprite, _data.SpriteName);
                return sprite;
            }

            // return null if failed to load resource somehow (tileset).
            return null;
        }

        private void AddSpriteToCache(Sprite _sprite, string _key)
        {
            if (_sprite != null)
            {
                spriteCache.Add(_key, _sprite);
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Trying to add null sprite to sprite cache using sprite name: '{_key}'. No resource " +
                               "exists with this sprite name");
            }
        }
    }
}

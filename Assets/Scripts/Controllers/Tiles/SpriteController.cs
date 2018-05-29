using System.Collections.Generic;
using Models.Map;
using Models.Sprites;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Abstract sprite controller class for setting sprites based on types.
    /// </summary>
	public abstract class SpriteController
    {
        /// <summary>
        /// Get the sprite for a specific tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
	    public abstract Sprite GetSprite(Tile _tile);

        /// <summary>
        /// Gets a given sprite from the sprite cache. If the sprite doesn't exist this function will NOT try load it from resources,
        /// and return null.
        /// </summary>
        /// <param name="_spriteName"></param>
        /// <returns></returns>
        protected Sprite GetSpriteFromCache(string _spriteName)
        {
            return SpriteCache.GetSprite(_spriteName);
        }

        /// <summary>
        /// Loads the given sprite at the given path to the sprite cache for the sprite controllers.
        /// </summary>
        /// <param name="_spritePath"></param>
        private static void LoadSprite(string _spritePath)
        {
            var sprite = Resources.Load<Sprite>(_spritePath);

            if(sprite == null)
            {
                Debug.LogError($"[SpriteController.LoadSprite] Failed to load sprite at path: {_spritePath}");
                return;
            }

            SpriteCache.AddSprite(sprite.name, sprite);
        }

        /// <summary>
        /// Loads the given tileset at the given path to the sprite cache for the sprite controllers.
        /// </summary>
        /// <param name="_tileSetPath"></param>
        private static void LoadTileSet(string _tileSetPath)
        {
            var tileset = Resources.LoadAll<Sprite>(_tileSetPath);

            if(tileset == null)
            {
                Debug.LogError($"[SpriteController.LoadTileSet] Failed to load tileset at path: {_tileSetPath}");
                return;
            }

            foreach(var sprite in tileset)
            {
                SpriteCache.AddSprite(sprite.name, sprite);
            }
        }

        public static void LoadSpriteData<T>(T _t) where T : ISpriteData
        {
            if(_t.GetIsTileSet())
            {
                LoadTileSet(_t.GetResourcesPath());
            }
            else
            {
                LoadSprite(_t.GetResourcesPath());
            }
        }
    }
}

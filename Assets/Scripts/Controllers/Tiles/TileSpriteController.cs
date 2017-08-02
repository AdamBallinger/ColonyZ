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
        protected static Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

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
            return spriteCache.ContainsKey(_spriteName) ? spriteCache[_spriteName] : null;
        }

        /// <summary>
        /// Loads the given sprite at the given path to the sprite cache for the sprite controllers.
        /// </summary>
        /// <param name="_spritePath"></param>
        public static void LoadSprite(string _spritePath)
        {
            var sprite = Resources.Load<Sprite>(_spritePath);

            if(sprite == null)
            {
                Debug.LogError($"[TileSpriteController.LoadSprite] Failed to load sprite at path: {_spritePath}");
                return;
            }

            spriteCache.Add(sprite.name, sprite);
        }

        /// <summary>
        /// Loads the given tileset at the given path to the sprite cache for the sprite controllers.
        /// </summary>
        /// <param name="_tileSetPath"></param>
        public static void LoadTileSet(string _tileSetPath)
        {
            var tileset = Resources.LoadAll<Sprite>(_tileSetPath);

            if(tileset == null)
            {
                Debug.LogError($"[TileSpriteController.LoadTileSet] Failed to load tileset at path: {_tileSetPath}");
                return;
            }

            foreach(var sprite in tileset)
            {
                spriteCache.Add(sprite.name, sprite);
            }
        }
    }
}

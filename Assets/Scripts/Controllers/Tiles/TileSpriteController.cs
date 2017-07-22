using Models.World;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Abstract Tile sprite controller class for setting tiles sprites based on types.
    /// </summary>
	public abstract class TileSpriteController : MonoBehaviour
	{
        /// <summary>
        /// Get the sprite for a specific tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
	    public abstract Sprite GetSprite(Tile _tile);

	}
}

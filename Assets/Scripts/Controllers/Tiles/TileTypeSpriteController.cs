using Models.Map;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Sprite controller that handles the setting of each tiles sprite based on its tile type.
    /// </summary>
	public class TileTypeSpriteController : TileSpriteController 
	{
	    public override Sprite GetSprite(Tile _tile)
	    {
            // TODO: When multiple types exist, such as dirt and stone, load the correct transitions when needed
            // using TileBitMask.
	        return GetSpriteFromCache(_tile.SpriteData.SpriteName);
	    }
	}
}

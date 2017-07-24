using Models.World;
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
	        return GetSpriteFromCache(_tile.SpriteData);
	    }
	}
}

﻿using Models.Map;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Sprite controller that handles the setting of each tiles sprite based on its tile type.
    /// </summary>
	public class TileTypeSpriteController : SpriteController 
	{
	    public override Sprite GetSprite(Tile _tile)
	    {
	        return GetSpriteFromCache(SpriteDataController.GetSpriteDataFor<TileSpriteData>(_tile.TileName).GetSpriteName());
	    }
	}
}

using Models.Map;
using Models.Sprites;
using UnityEngine;

namespace Controllers.Tiles
{
	public class TileTypeSpriteController : SpriteController 
	{
	    public override Sprite GetSprite(Tile _tile)
	    {
	        if (_tile.Type == TileType.None) return null;

	        return GetSpriteFromCache(SpriteDataController.GetSpriteDataFor<TileSpriteData>(_tile.TileName).GetSpriteName());
	    }
	}
}

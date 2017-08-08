using Models.Map;
using UnityEngine;

namespace Controllers.Tiles
{
    /// <summary>
    /// Sprite controller for handling settings the sprite for structures placed on tiles such as walls.
    /// </summary>
	public class TileStructureSpriteController : TileSpriteController 
	{
	    public override Sprite GetSprite(Tile _tile)
	    {
	        var tileStructure = _tile.InstalledStructure;

            if(tileStructure != null)
            {
                var structureSpriteData = tileStructure.SpriteData;

                if(tileStructure.Type.Equals(TileStructureType.Multi_Tile))
                {
                    var bitmask = TileBitMask.ComputeBitmaskValue(_tile, BitmaskEvaluationType.Tile_Structure);

                    return GetSpriteFromCache(structureSpriteData.SpriteName + bitmask);
                }

                // If not multi-tile, then just return the sprite name inside the structure sprite data.
                return GetSpriteFromCache(structureSpriteData.SpriteName);
            }

	        return null;
	    }
	}
}

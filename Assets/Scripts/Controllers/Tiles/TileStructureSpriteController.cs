using Models.World;
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

                if(tileStructure.Type.Equals(TileStructureType.Wall))
                {
                    var bitmask = TileBitMask.ComputeBitmaskValue(_tile);
                    var bitMaskIndex = TileBitMask.bitMaskMap[bitmask];

                    return GetSpriteFromCache(structureSpriteData.SpriteName + bitMaskIndex);
                }

                // TODO: Standard object sprite fetching.
                
            }

	        return null;
	    }
	}
}

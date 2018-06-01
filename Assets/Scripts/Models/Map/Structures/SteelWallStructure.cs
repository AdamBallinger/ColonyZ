using Controllers;
using UnityEngine;

namespace Models.Map.Structures
{
    public class SteelWallStructure : TileStructure
    {
        public SteelWallStructure(string _structureName) : base(_structureName)
        {
            Type = TileStructureType.Multi_Tile;
            ConnectsToSelf = true;
        }
        public override TileStructure Clone()
        {
            var clone = new SteelWallStructure(StructureName);
            CopyInto(clone);
            return clone;
        }

        public override Sprite GetIcon()
        {
            var sprite = SpriteCache.GetSprite(SpriteData.GetSpriteName() + 47);
            return sprite;
        }
    }
}

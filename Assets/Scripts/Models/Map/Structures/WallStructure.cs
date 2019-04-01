using Controllers;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Structures
{
    public class WallStructure : TileStructure
    {
        public WallStructure(string _structureName, params string[] _connectsTo) : base(_structureName)
        {
            Type = TileStructureType.Multi_Tile;
            ConnectsToSelf = true;

            Connectables.AddRange(_connectsTo);
        }

        public override TileStructure Clone()
        {
            var clone = new WallStructure(StructureName);
            CopyInto(clone);
            return clone;
        }

        public override bool CanPlace(Tile _tile)
        {
            return _tile.Structure == null;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, 47);
        }
    }
}

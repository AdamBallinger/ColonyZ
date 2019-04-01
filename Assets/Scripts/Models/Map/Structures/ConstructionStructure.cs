using Controllers;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Structures
{
    public class ConstructionStructure : TileStructure
    {
        public ConstructionStructure(string _structureName) : base(_structureName)
        {
            Type = TileStructureType.Multi_Tile;
            Enterability = TileEnterability.Immediate;
            MovementModifier = 1.0f;
            ConnectsToSelf = true;
        }
        
        public override TileStructure Clone()
        {
            var clone = new ConstructionStructure(StructureName);
            CopyInto(clone);
            return clone;
        }

        public override bool CanPlace(Tile _tile)
        {
            return _tile?.Structure == null;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite("Construction_Base", 47);
        }
    }
}
using Controllers;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public class ConstructionObject : TileObject
    {
        public ConstructionObject(string _structureName) : base(_structureName)
        {
            Type = TileObjectType.Multi_Tile;
            Enterability = TileEnterability.Immediate;
            MovementModifier = 1.0f;
            ConnectsToSelf = true;
        }
        
        public override TileObject Clone()
        {
            var clone = new ConstructionObject(StructureName);
            CopyInto(clone);
            return clone;
        }

        public override bool CanPlace(Tile _tile)
        {
            return _tile?.Object == null;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite("Construction_Base", 47);
        }
    }
}
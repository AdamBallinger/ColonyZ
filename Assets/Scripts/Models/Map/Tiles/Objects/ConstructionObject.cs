using Controllers;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public class ConstructionObject : TileObject
    {
        public ConstructionObject(string _objectName) : base(_objectName)
        {
            Type = TileObjectType.Multi_Tile;
            Enterability = TileEnterability.Immediate;
            MovementModifier = 1.0f;
            ConnectsToSelf = true;
        }
        
        public override TileObject Clone()
        {
            var clone = new ConstructionObject(ObjectName);
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
using Controllers;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public class WallObject : TileObject
    {
        public WallObject(string _structureName, params string[] _connectsTo) : base(_structureName)
        {
            Type = TileObjectType.Multi_Tile;
            ConnectsToSelf = true;

            Connectables.AddRange(_connectsTo);
        }

        public override TileObject Clone()
        {
            var clone = new WallObject(StructureName);
            CopyInto(clone);
            return clone;
        }

        public override bool CanPlace(Tile _tile)
        {
            return _tile.Object == null;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, 47);
        }
    }
}

using Controllers;
using UnityEngine;

namespace Models.Map.Structures
{
    public class DoorStructure : TileStructure
    {
        private readonly string iconName;

        public DoorStructure(string _structureName, string _iconName) : base(_structureName)
        {
            Type = TileStructureType.Single_Tile;
            iconName = _iconName;
            SpriteData = SpriteDataController.GetSpriteData("Doors");
        }

        public override TileStructure Clone()
        {
            var clone = new DoorStructure(StructureName, iconName);
            CopyInto(clone);
            return clone;
        }

        public override int GetSpriteIndex()
        {
            var east = World.Instance.GetTileAt(Tile.X + 1, Tile.Y);
            var west = World.Instance.GetTileAt(Tile.X - 1, Tile.Y);

            if (east != null && west != null)
            {
                if (east.Structure != null && east.Structure.GetType() == typeof(WallStructure) &&
                   west.Structure != null && west.Structure.GetType() == typeof(WallStructure))
                {
                    return 0;
                }
            }

            var north = World.Instance.GetTileAt(Tile.X, Tile.Y + 1);
            var south = World.Instance.GetTileAt(Tile.X, Tile.Y - 1);

            if (north != null && south != null)
            {
                if (north.Structure != null && north.Structure.GetType() == typeof(WallStructure) &&
                   south.Structure != null && south.Structure.GetType() == typeof(WallStructure))
                {
                    return 1;
                }
            }

            return 0;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, iconName);
        }
    }
}

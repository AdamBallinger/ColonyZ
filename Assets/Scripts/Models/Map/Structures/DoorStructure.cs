using Controllers;
using UnityEngine;

namespace Models.Map.Structures
{
    public class DoorStructure : TileStructure
    {
        public DoorStructure(string _structureName) : base(_structureName)
        {
            Type = TileStructureType.Single_Tile;
        }

        public override TileStructure Clone()
        {
            var clone = new DoorStructure(StructureName);
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

        public override bool CanPlace(Tile _tile)
        {
            var east = World.Instance.GetTileAt(_tile.X + 1, _tile.Y);
            var west = World.Instance.GetTileAt(_tile.X - 1, _tile.Y);
            var north = World.Instance.GetTileAt(_tile.X, _tile.Y + 1);
            var south = World.Instance.GetTileAt(_tile.X, _tile.Y - 1);

            if (east != null && west != null)
            {
                if (east.Structure != null && east.Structure.GetType() == typeof(WallStructure) &&
                    west.Structure != null && west.Structure.GetType() == typeof(WallStructure))
                {
                    if(north?.Structure == null && south?.Structure == null)
                    {
                        return true;
                    }
                }
            }

            if(north != null && south != null)
            {
                if(north.Structure != null && north.Structure.GetType() == typeof(WallStructure) &&
                   south.Structure != null && south.Structure.GetType() == typeof(WallStructure))
                {
                    if(east?.Structure == null && west?.Structure == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, 0);
        }
    }
}

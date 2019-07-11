using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Door_", menuName = "ColonyZ/Door Object", order = 51)]
    public class DoorObject : TileObject
    {   
        public override int GetSpriteIndex()
        {
            var east = World.Instance.GetTileAt(Tile.X + 1, Tile.Y);
            var west = World.Instance.GetTileAt(Tile.X - 1, Tile.Y);

            if (east != null && west != null)
            {
                if (east.Object != null && east.Object.GetType() == typeof(WallObject) &&
                   west.Object != null && west.Object.GetType() == typeof(WallObject))
                {
                    return 0;
                }
            }

            var north = World.Instance.GetTileAt(Tile.X, Tile.Y + 1);
            var south = World.Instance.GetTileAt(Tile.X, Tile.Y - 1);

            if (north != null && south != null)
            {
                if (north.Object != null && north.Object.GetType() == typeof(WallObject) &&
                   south.Object != null && south.Object.GetType() == typeof(WallObject))
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
                if (east.Object != null && east.Object.GetType() == typeof(WallObject) &&
                    west.Object != null && west.Object.GetType() == typeof(WallObject))
                {
                    if(north?.Object == null && south?.Object == null)
                    {
                        return true;
                    }
                }
            }

            if(north != null && south != null)
            {
                if(north.Object != null && north.Object.GetType() == typeof(WallObject) &&
                   south.Object != null && south.Object.GetType() == typeof(WallObject))
                {
                    if(east?.Object == null && west?.Object == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

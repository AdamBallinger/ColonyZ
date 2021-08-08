using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects.Data
{
    [CreateAssetMenu(menuName = "ColonyZ/Door Object Data", fileName = "Door Data")]
    public class DoorObjectData : TileObjectData
    {
        public override bool CanPlace(Tile _tile)
        {
            if (_tile.HasObject) return false;

            var east = World.Instance.GetTileAt(_tile.X + 1, _tile.Y);
            var west = World.Instance.GetTileAt(_tile.X - 1, _tile.Y);
            var north = World.Instance.GetTileAt(_tile.X, _tile.Y + 1);
            var south = World.Instance.GetTileAt(_tile.X, _tile.Y - 1);

            if (east != null && west != null)
                if (east.Object != null && east.Object.ObjectData.EnclosesRoom && 
                    west.Object != null && west.Object.ObjectData.EnclosesRoom)
                    if (north?.Object == null && south?.Object == null)
                        return true;
            
            if (north != null && south != null)
                if (north.Object != null && north.Object.ObjectData.EnclosesRoom && 
                    south.Object != null && south.Object.ObjectData.EnclosesRoom)
                    if (east?.Object == null && west?.Object == null)
                        return true;

            return false;
        }
    }
}
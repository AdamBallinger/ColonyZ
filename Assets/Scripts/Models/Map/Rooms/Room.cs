using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Rooms
{
    public class Room
    {
        /// <summary>
        /// Unique ID for this room.
        /// </summary>
        public int RoomID => RoomManager.Instance.GetRoomID(this);
        
        /// <summary>
        /// List of tiles that assigned to this room. This does not include the tiles that enclose the room.
        /// </summary>
        public List<Tile> Tiles { get; }

        public Room()
        {
            Tiles = new List<Tile>();
        }
        
        public void AssignTile(Tile _tile)
        {
            if (Tiles.Contains(_tile)) return;

            // Remove tile from its previous room.
            _tile.Room?.UnassignTile(_tile);

            Tiles.Add(_tile);
            _tile.Room = this;
        }
        
        /// <summary>
        /// Removes the tile from the room, and gives the tile a null room.
        /// </summary>
        /// <param name="_tile"></param>
        public void UnassignTile(Tile _tile)
        {
            if (!Tiles.Contains(_tile)) return;

            Tiles.Remove(_tile);
            _tile.Room = null;
        }

        /// <summary>
        /// Place all tiles in this room back into the outside room.
        /// </summary>
        public void ReleaseTilesToOutside()
        {
            for (var i = Tiles.Count - 1; i >= 0; i--)
            {
                RoomManager.Instance.OutsideRoom.AssignTile(Tiles[i]);
            }
        }
    }
}
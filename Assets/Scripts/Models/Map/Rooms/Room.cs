using System.Collections.Generic;
using System.Linq;
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
        public HashSet<Tile> Tiles { get; }

        public HashSet<Room> ConnectedRooms { get; }

        public Room()
        {
            Tiles = new HashSet<Tile>();
            ConnectedRooms = new HashSet<Room>();
        }

        public void AssignTile(Tile _tile)
        {
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
            Tiles.Remove(_tile);
            _tile.Room = null;
        }

        /// <summary>
        /// Unassign all tiles in the room.
        /// </summary>
        public void ReleaseTiles()
        {
            var tiles = Tiles.ToList();
            for (var i = Tiles.Count - 1; i >= 0; i--)
            {
                tiles[i].Room?.UnassignTile(tiles[i]);
            }
        }

        public void AddConnection(Room _room)
        {
            if (_room == null || ConnectedRooms.Contains(_room)) return;

            ConnectedRooms.Add(_room);
        }

        public bool HasConnection(Room _room)
        {
            return ConnectedRooms.Contains(_room);
        }
    }
}
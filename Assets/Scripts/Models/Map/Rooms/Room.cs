using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Rooms
{
    public class Room
    {
        /// <summary>
        /// Unique ID for this room.
        /// </summary>
        public int RoomID { get; }
        
        /// <summary>
        /// List of tiles that assigned to this room. This does not include the tiles that enclose the room.
        /// </summary>
        public List<Tile> Tiles { get; }
        
        /// <summary>
        /// List of rooms that have a direct entrance to this room.
        /// </summary>
        public List<Room> ConnectedRooms { get; }
        
        public Room(int _id, List<Tile> _tiles)
        {
            RoomID = _id;
            Tiles = _tiles;
            ConnectedRooms = new List<Room>();
        }
        
        public void AssignTile(Tile _tile)
        {
            if (Tiles.Contains(_tile)) return;
            
            Tiles.Add(_tile);
            _tile.Room = this;
        }
    }
}
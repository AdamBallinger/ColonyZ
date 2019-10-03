using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Rooms
{
    public class RoomManager
    {
        public static RoomManager Instance { get; private set; }

        public static int OUTSIDE_ROOM_ID => 0;
        
        public Room OutsideRoom { get; }
        
        private List<Room> Rooms { get; }
        
        private RoomManager()
        {
            OutsideRoom = new Room();
            Rooms = new List<Room>
            {
                OutsideRoom
            };
        }
        
        public static void Create()
        {
            if (Instance != null) return;
            
            Instance = new RoomManager();
        }
        
        public int GetRoomID(Room _room)
        {
            return Rooms.IndexOf(_room);
        }
        
        public void RemoveRoom(Room _room)
        {
            if (_room == null || _room != null && _room.RoomID == OUTSIDE_ROOM_ID) return;

            _room?.ReleaseTilesToOutside();
            Rooms.Remove(_room);
        }

        public void CheckForRoom(Tile _tile)
        {
            var oldRoom = _tile.Room;
            
            // An enclosing object was built on this tile.
            if (oldRoom != null)
            {
                // Flood each neighbour to see if any are now enclosed.
                foreach (var tile in _tile.DirectNeighbours)
                {
                    FloodFill(tile, oldRoom);
                }

                // Remove the source tile from its current room, as enclosing tiles do not belong to any rooms.
                oldRoom.UnassignTile(_tile);
                
                // Delete the old source tile room as it is no longer needed (Unless it was the outside room).
                RemoveRoom(oldRoom);
            }
            else
            {
                // Getting here means the tile previously had an enclosing object (Wall, door etc.) on it
                // So go through each of the neighbour tiles and remove their rooms, as it means we could potentially be merging
                // up to 4 rooms together.
                foreach (var tile in _tile.Neighbours)
                {
                    RemoveRoom(tile.Room);
                }
                
                // Once the rooms are removed, flood from the source tile to create the new room.
                FloodFill(_tile, null);
            }
        }
        
        /// <summary>
        /// Perform a flood fill on a given tile to check if a new room needs to be created.
        /// </summary>
        /// <param name="_startTile"></param>
        /// <param name="_oldRoom"></param>
        private void FloodFill(Tile _startTile, Room _oldRoom)
        {
            // Can't create a room outside of the map..
            if (_startTile == null) return;

            // Tile has an enclosing object so a room can't be made here.
            if (_startTile.HasObject && _startTile.Object.EnclosesRoom) return;

            // The tile must have already been assigned a new room so skip this tile.
            if (_startTile.Room != _oldRoom) return;

            var newRoom = new Room();
            
            var tilesToCheck = new Queue<Tile>();
            tilesToCheck.Enqueue(_startTile);

            var connectedToOutside = false;

            while (tilesToCheck.Count > 0)
            {
                var currentTile = tilesToCheck.Dequeue();

                if (currentTile.Room != newRoom)
                {
                    newRoom.AssignTile(currentTile);
                    
                    foreach (var neighbour in currentTile.DirectNeighbours)
                    {
                        // This neighbour has already been added to the new room.
                        if (neighbour.Room == newRoom) continue;
                        
                        // Enclosing objects don't get assigned to rooms.
                        if (neighbour.HasObject && neighbour.Object.EnclosesRoom) continue;
                        
                        if (neighbour.IsExposedToOutside())
                        {
                            // Room isn't valid as it is not enclosed.
                            connectedToOutside = true;
                        }

                        tilesToCheck.Enqueue(neighbour);
                    }
                    
                    if (connectedToOutside)
                    {
                        newRoom.ReleaseTilesToOutside();
                        return;
                    }
                }
            }
            
            Rooms.Add(newRoom);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Utils;

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

            OutsideRoom.AddConnection(OutsideRoom);
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

        private void RemoveRoom(Room _room)
        {
            if (_room == null || _room.RoomID == OUTSIDE_ROOM_ID) return;

            _room.ReleaseTilesToOutside();
            Rooms.Remove(_room);
        }

        public void CheckForRoom(Tile _tile)
        {
            // Don't try flood from world edge tile.
            if (_tile.IsMapEdge) return;

            // Don't flood from tiles that enclose areas, but are not buildable, such as trees.
            if (_tile.HasObject && _tile.Object.EnclosesRoom && !_tile.Object.Buildable) return;

            var oldRoom = _tile.Room;

            Predicate<Tile> floodfill_ConditionCheck = t => t != null
                                                            && t.Room == oldRoom
                                                            && !(t.HasObject && t.Object.EnclosesRoom);

            Predicate<Tile> floodfill_PassCheck = t => t != null
                                                       && t.Room == oldRoom
                                                       && !t.ExposedToMapBottom();

            // An enclosing object was built on this tile.
            if (oldRoom != null)
            {
                // Flood each neighbour to see if any are now enclosed.
                foreach (var tile in _tile.DirectNeighbours)
                {
                    FloodFiller.Flood(tile,
                        floodfill_ConditionCheck,
                        floodfill_PassCheck,
                        set => CreateRoom(set.ToList()));
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
                foreach (var tile in _tile.DirectNeighbours)
                {
                    RemoveRoom(tile.Room);
                }

                // Place the tile that currently has no room assigned back to the outside for now.
                OutsideRoom.AssignTile(_tile);

                // Set the target room for the flood fill to find tiles that are marked as outside.
                oldRoom = OutsideRoom;

                // Old rooms removed, and source tile released to outside, flood from it.
                FloodFiller.Flood(_tile,
                    floodfill_ConditionCheck,
                    floodfill_PassCheck,
                    set => CreateRoom(set.ToList()));
            }

            GenerateRoomConnections();
        }

        private void CreateRoom(List<Tile> _tiles)
        {
            if (_tiles != null && _tiles.Count > 0)
            {
                var room = new Room();
                _tiles.ForEach(t => room.AssignTile(t));
                Rooms.Add(room);
            }
        }

        private void GenerateRoomConnections()
        {
            var doors = World.Instance.Objects.OfType<DoorObject>().ToList();

            // Clear existing connections, then re-add self connections.
            foreach (var room in Rooms)
            {
                room.ConnectedRooms.Clear();
                room.AddConnection(room);
            }

            foreach (var door in doors)
            {
                var tile = door.Tile;
                var n = World.Instance.GetTileAt(tile.X, tile.Y + 1);
                var s = World.Instance.GetTileAt(tile.X, tile.Y - 1);

                // if the tiles to the north and south of the door have different room ids, then they are connected.
                if (n != null && s != null && n.Room?.RoomID != s.Room?.RoomID)
                {
                    n.Room?.AddConnection(s.Room);
                    s.Room?.AddConnection(n.Room);
                    continue;
                }

                var e = World.Instance.GetTileAt(tile.X + 1, tile.Y);
                var w = World.Instance.GetTileAt(tile.X - 1, tile.Y);

                if (e != null && w != null && e.Room?.RoomID != w.Room?.RoomID)
                {
                    e.Room?.AddConnection(w.Room);
                    w.Room?.AddConnection(e.Room);
                }
            }

            foreach (var room in Rooms)
            {
                var checkedRooms = new List<Room>();
                var rooms = LinkRoom(room, checkedRooms);

                foreach (var r in rooms)
                {
                    room.AddConnection(r);
                }
            }
        }

        private List<Room> LinkRoom(Room _room, List<Room> _checkedRooms)
        {
            foreach (var room in _room.ConnectedRooms)
            {
                if (_checkedRooms.Contains(room)) continue;

                _checkedRooms.Add(room);
                LinkRoom(room, _checkedRooms);
            }

            return _checkedRooms;
        }
    }
}
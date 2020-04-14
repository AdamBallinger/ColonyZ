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

        public List<Room> Rooms { get; }

        /// <summary>
        /// Event called after new rooms have been created, or old rooms deleted.
        /// </summary>
        public event Action roomsUpdatedEvent;

        private bool shouldTriggerUpdate = false;

        private RoomManager()
        {
            Rooms = new List<Room>();
        }

        public static void Create()
        {
            if (Instance != null) return;

            Instance = new RoomManager();
        }

        public int GetRoomID(Room _room)
        {
            return Rooms.IndexOf(_room) + 1;
        }

        private void RemoveRoom(Room _room)
        {
            if (_room == null) return;

            _room.ReleaseTiles();
            Rooms.Remove(_room);
            shouldTriggerUpdate = true;
        }

        public void CheckForRoom(Tile _tile)
        {
            if (_tile.IsMapEdge) return;

            var oldRoom = _tile.Room;

            Predicate<Tile> floodfill_ConditionCheck = t => t != null
                                                            && t.Room == oldRoom
                                                            && !(t.HasObject && t.Object.EnclosesRoom);

            Predicate<Tile> floodfill_PassCheck = t => t.Room == oldRoom;

            // An enclosing object was built on this tile.
            if (oldRoom != null)
            {
                // TODO: Try optimise this so it doesnt release all tiles in the room it was placed.
                // Causes massive slow downs on large maps. This is a downside to not having an
                // outside room system.. but its going to make stuff so much easier if this works..
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

                // Delete the old source tile room as it is no longer needed.
                RemoveRoom(oldRoom);
            }
            else
            {
                // Getting here means the tile previously had an enclosing object (Wall, door etc.) on it
                // So go through each of the neighbour tiles and remove their rooms, as it means we could
                // potentially be merging up to 4 rooms together.
                foreach (var tile in _tile.DirectNeighbours)
                {
                    RemoveRoom(tile.Room);
                }

                // Old rooms removed, flood from source to find new rooms.
                FloodFiller.Flood(_tile,
                    floodfill_ConditionCheck,
                    floodfill_PassCheck,
                    set => CreateRoom(set.ToList()));
            }

            if (shouldTriggerUpdate)
            {
                roomsUpdatedEvent?.Invoke();
                shouldTriggerUpdate = false;
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
                shouldTriggerUpdate = true;
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
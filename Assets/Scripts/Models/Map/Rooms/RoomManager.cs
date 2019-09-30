using System.Collections.Generic;
using Models.Map.Tiles;

namespace Models.Map.Rooms
{
    public class RoomManager
    {
        public static RoomManager Instance { get; private set; }
        
        public Room OutsideRoom { get; }
        
        private RoomManager()
        {
            OutsideRoom = new Room(0, new List<Tile>());
        }
        
        public static void Create()
        {
            if (Instance != null) return;
            
            Instance = new RoomManager();
        }
    }
}
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.TimeSystem;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    public class DoorObject : TileObject
    {
        public bool IsOpen { get; private set; }

        private bool isOpening;

        /// <summary>
        ///     Current time spent waiting to open door.
        /// </summary>
        private float currentOpeningTime;
        
        /// <summary>
        ///     Total time required for the door to open.
        /// </summary>
        private float openDelay = 0.5f;
        
        /// <summary>
        ///     Current time the door has been open for.
        /// </summary>
        private float currentOpenTime;
        
        /// <summary>
        ///     Max time the door can remain open for.
        /// </summary>
        private float maxOpenTime = 1.25f;
        
        public DoorObject(TileObjectData _data) : base(_data)
        {
        }
        
        public override int GetSpriteIndex()
        {
            var east = World.Instance.GetTileAt(OriginTile.X + 1, OriginTile.Y);
            var west = World.Instance.GetTileAt(OriginTile.X - 1, OriginTile.Y);

            if (east != null && west != null)
                if (east.Object != null && east.Object.ObjectData.EnclosesRoom &&
                    west.Object != null && west.Object.ObjectData.EnclosesRoom)
                    return IsOpen ? 1 : 0;
            
            var north = World.Instance.GetTileAt(OriginTile.X, OriginTile.Y + 1);
            var south = World.Instance.GetTileAt(OriginTile.X, OriginTile.Y - 1);
            
            if (north != null && south != null)
                if (north.Object != null && north.Object.ObjectData.EnclosesRoom &&
                    south.Object != null && south.Object.ObjectData.EnclosesRoom)
                    return IsOpen ? 3 : 2;

            return 0;
        }

        public void OpenDoor()
        {
            if (!isOpening && !IsOpen)
            {
                isOpening = true;
                currentOpeningTime = 0.0f;
            }

            // Reset the open timer if an entity attempts to open the door whilst already open.
            if (IsOpen)
            {
                currentOpenTime = 0.0f;
            }
        }

        public override void Update()
        {
            if (isOpening)
            {
                currentOpeningTime += TimeManager.Instance.DeltaTime;

                if (currentOpeningTime >= openDelay)
                {
                    IsOpen = true;
                    isOpening = false;
                    OriginTile.MarkDirty();
                }
            }
            
            if (IsOpen)
            {
                currentOpenTime += TimeManager.Instance.DeltaTime;

                if (currentOpenTime >= maxOpenTime && OriginTile.LivingEntities.Count == 0)
                {
                    IsOpen = false;
                    currentOpenTime = 0.0f;
                    OriginTile.MarkDirty();
                }
            }
        }
    }
}
using ColonyZ.Models.TimeSystem;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects
{
    [CreateAssetMenu(fileName = "TileObject_Door_", menuName = "ColonyZ/Door Object", order = 51)]
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
        
        public override int GetSpriteIndex()
        {
            var east = World.Instance.GetTileAt(Tile.X + 1, Tile.Y);
            var west = World.Instance.GetTileAt(Tile.X - 1, Tile.Y);

            if (east != null && west != null)
                if (east.HasObject && east.Object.GetType() == typeof(WallObject) &&
                    west.HasObject && west.Object.GetType() == typeof(WallObject))
                    return IsOpen ? 1 : 0;

            var north = World.Instance.GetTileAt(Tile.X, Tile.Y + 1);
            var south = World.Instance.GetTileAt(Tile.X, Tile.Y - 1);

            if (north != null && south != null)
                if (north.HasObject && north.Object.GetType() == typeof(WallObject) &&
                    south.HasObject && south.Object.GetType() == typeof(WallObject))
                    return IsOpen ? 3 : 2;

            return 0;
        }

        public override bool CanPlace(Tile _tile)
        {
            if (!base.CanPlace(_tile)) return false;

            var east = World.Instance.GetTileAt(_tile.X + 1, _tile.Y);
            var west = World.Instance.GetTileAt(_tile.X - 1, _tile.Y);
            var north = World.Instance.GetTileAt(_tile.X, _tile.Y + 1);
            var south = World.Instance.GetTileAt(_tile.X, _tile.Y - 1);

            if (east != null && west != null)
                if (east.HasObject && east.Object.GetType() == typeof(WallObject) &&
                    west.HasObject && west.Object.GetType() == typeof(WallObject))
                    if (north?.Object == null && south?.Object == null)
                        return true;

            if (north != null && south != null)
                if (north.HasObject && north.Object.GetType() == typeof(WallObject) &&
                    south.HasObject && south.Object.GetType() == typeof(WallObject))
                    if (east?.Object == null && west?.Object == null)
                        return true;

            return false;
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
                    Tile.MarkDirty();
                }
            }
            
            if (IsOpen)
            {
                currentOpenTime += TimeManager.Instance.DeltaTime;

                if (currentOpenTime >= maxOpenTime && Tile.LivingEntities.Count == 0)
                {
                    IsOpen = false;
                    currentOpenTime = 0.0f;
                    Tile.MarkDirty();
                }
            }
        }
    }
}
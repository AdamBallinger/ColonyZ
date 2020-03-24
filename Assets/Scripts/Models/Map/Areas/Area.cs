using Models.Map.Rooms;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Areas
{
    public abstract class Area
    {
        public string AreaName { get; protected set; }

        public bool RequiresRoom { get; protected set; }

        // TODO: Maybe change this so that the tile with an object just isn't a part of the area?
        /// <summary>
        /// Determines if the area can contain objects when being placed in the word.
        /// This does not affect the area when an object is being built after the area is placed.
        /// </summary>
        public bool CanContainObjects { get; protected set; }

        public Vector2 MinimumSize { get; protected set; }

        public Vector2 Origin { get; protected set; }

        public Vector2 Size { get; protected set; }

        protected Area()
        {
            MinimumSize = Vector2.one;
        }

        public virtual void SetOrigin(int _x, int _y)
        {
            Origin = new Vector2(_x, _y);
        }

        public virtual void SetSize(int _width, int _height)
        {
            Size = new Vector2(_width, _height);
        }

        public virtual bool IsValidSize()
        {
            return Size.x >= MinimumSize.x && Size.y >= MinimumSize.y;
        }

        public virtual bool CanPlace(Tile _tile)
        {
            if (_tile.Area != null) return false;
            if (RequiresRoom && _tile.Room.RoomID == RoomManager.OUTSIDE_ROOM_ID) return false;
            if (_tile.HasObject && !CanContainObjects) return false;

            return true;
        }
    }
}
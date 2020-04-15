using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Zones
{
    public abstract class Zone
    {
        public string ZoneName { get; protected set; }

        // TODO: Maybe change this so that the tile with an object just isn't a part of the zone?
        /// <summary>
        /// Determines if the zone can contain objects when being placed in the word.
        /// This does not affect the zone when an object is being built after the zone is placed.
        /// </summary>
        public bool CanContainObjects { get; protected set; }

        public Vector2 MinimumSize { get; protected set; }

        public Vector2 Origin { get; protected set; }

        public Vector2 Size { get; protected set; }

        protected Zone()
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

        public virtual bool CanPlace(Tile _tile)
        {
            if (_tile.Zone != null) return false;
            if (_tile.HasObject && !CanContainObjects) return false;

            return true;
        }
    }
}
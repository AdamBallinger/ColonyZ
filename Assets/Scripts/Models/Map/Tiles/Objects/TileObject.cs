using Models.Sprites;
using Models.UI;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public abstract class TileObject : ScriptableObject, ISelectable
    {
        /// <summary>
        /// The Tile this object originates from. If the object is a multi tile object, then this is the "base" tile
        /// for that object.
        /// </summary>
        public Tile OriginTile { get; set; }

        /// <summary>
        /// The Tile this part of a object occupies. If the object is a single type, then this will be the same as OriginTile.
        /// </summary>
        public Tile Tile { get; set; }

        public SpriteData SpriteData => objectSpriteData;

        public bool DynamicSprite => dynamicSprite;

        /// <summary>
        /// Name of the object.
        /// </summary>
        public string ObjectName => objectName;

        /// <summary>
        /// Determines if the object can be placed over a dragged area, or one at a time.
        /// </summary>
        public bool Draggable => draggable;

        /// <summary>
        /// Determines if the object appears in the structure menu.
        /// </summary>
        public bool Buildable => buildable;

        public bool Fellable => fellable;

        public bool Mineable => mineable;

        public bool Harvestable => harvestable;

        public bool EnclosesRoom => enclosesRoom;

        public int Width => objectWidth;
        public int Height => objectHeight;

        public TileEnterability Enterability => objectEnterability;

        /// <summary>
        /// Returns whether this object occupies more than 1 tile.
        /// </summary>
        public bool MultiTile => Width > 1 || Height > 1;

        #region Serialized fields

        [SerializeField] private SpriteData objectSpriteData;

        [SerializeField,
         Tooltip("Used for tiles that are 1,1 in size, but have dynamically changing sprites based on surroundings.")]
        private bool dynamicSprite;

        [SerializeField] private string objectName;

        [SerializeField, Tooltip("If enabled, then a preview for the object will be shown over dragged area.")]
        private bool draggable = true;

        [SerializeField] private bool buildable = true;

        [SerializeField] private bool fellable;

        [SerializeField] private bool mineable;

        [SerializeField] private bool harvestable;

        [SerializeField] private bool enclosesRoom;

        [SerializeField] private int objectWidth = 1;
        [SerializeField] private int objectHeight = 1;

        [SerializeField, Tooltip("Controls how this object affects AI pathing of the tile it occupies.")]
        private TileEnterability objectEnterability;

        #endregion

        public virtual void Update()
        {
        }

        /// <summary>
        /// Returns if this object connects to a given object. By default, no objects connect to each other.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public virtual bool ConnectsWith(TileObject _other)
        {
            return false;
        }

        /// <summary>
        /// Returns the sprite index to use for single tile structures. This does nothing for multi tile or dynamic tile objects.
        /// </summary>
        /// <returns></returns>
        public virtual int GetSpriteIndex()
        {
            return 0;
        }

        public virtual int GetSortingOrder()
        {
            return World.Instance.Height - Tile.Y;
        }

        /// <summary>
        /// Checks if the structure can be placed on the given tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public abstract bool CanPlace(Tile _tile);

        /// <summary>
        /// Return an icon sprite for this tile structure. This is used to display the structure in UI.
        /// </summary>
        /// <returns></returns>
        public Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, SpriteData.IconIndex);
        }

        #region ISelectable Implementation

        public Sprite GetSelectionIcon()
        {
            return GetIcon();
        }

        public string GetSelectionName()
        {
            return objectName;
        }

        public string GetSelectionDescription()
        {
            return Tile.GetSelectionDescription() +
                   "Durability: 0/0\n";
        }

        public Vector2 GetPosition()
        {
            return Tile.Position;
        }

        #endregion
    }
}
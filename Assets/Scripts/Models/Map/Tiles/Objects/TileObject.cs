using Controllers;
using Models.Sprites;
using UnityEngine;

namespace Models.Map.Tiles.Objects
{
    public abstract class TileObject : ScriptableObject
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

        public int Width => objectWidth;
        public int Height => objectHeight;

        public TileEnterability Enterability => objectEnterability;

        /// <summary>
        /// Returns whether this object occupies more than 1 tile.
        /// </summary>
        public bool MultiTile => Width > 1 || Height > 1;

        #region Serialized fields

        [SerializeField]
        private SpriteData objectSpriteData;

        [SerializeField, Tooltip("Used for tiles that are 1,1 in size, but have dynamically changing sprites based on surroundings.")]
        private bool dynamicSprite;

        [SerializeField]
        private string objectName;

        [SerializeField]
        private int objectWidth = 1;
        [SerializeField]
        private int objectHeight = 1;
        
        [SerializeField, Tooltip("Controls how this object affects AI pathing of the tile it occupies.")]
        private TileEnterability objectEnterability;
        
        #endregion
        
        public virtual void Update() {}

        /// <summary>
        /// Returns if this structure connects to a given structure.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public virtual bool ConnectsWith(TileObject _other)
        {
            // TODO: Implement this in appropriate object classes so dynamic sprites work.
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

        /// <summary>
        /// Checks if the structure can be placed on the given tile.
        /// </summary>
        /// <param name="_tile"></param>
        /// <returns></returns>
        public abstract bool CanPlace(Tile _tile);

        /// <summary>
        /// Return an icon sprite for this tile structure. This is used for display the structure in UI.
        /// </summary>
        /// <returns></returns>
        public Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, SpriteData.IconIndex);
        }
    }
}

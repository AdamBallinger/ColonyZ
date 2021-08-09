using ColonyZ.Models.Map.Tiles.Objects.Factory;
using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Models.Map.Tiles.Objects.Data
{
    [CreateAssetMenu(menuName = "ColonyZ/Object Data", fileName = "Object Data")]
    public class TileObjectData : ScriptableObject
    {
        [SerializeField] private ObjectFactoryType factoryType;
        [SerializeField] private string objectName;
        
        [SerializeField] private SpriteData spriteData;
        [SerializeField] private bool smartObject;
        [SerializeField] private bool buildable;
        [SerializeField] private bool rotatable;
        [SerializeField] private bool enlosesRoom;

        [SerializeField] private int objectWidth;
        [SerializeField] private int objectHeight;

        [SerializeField] private TileEnterability enterability;

        public ObjectFactoryType FactoryType => factoryType;

        public string ObjectName => objectName;
        
        public SpriteData SpriteData => spriteData;
        public bool SmartObject => smartObject;
        public bool Buildable => buildable;
        public bool Rotatable => rotatable;
        public bool EnclosesRoom => enlosesRoom;

        public int ObjectWidth => objectWidth;
        public int ObjectHeight => objectHeight;

        public TileEnterability Enterability => enterability;

        public bool MultiTile => objectWidth > 1 || objectHeight > 1;
        public bool Draggable => !MultiTile;
        
        /// <summary>
        ///     Return an icon sprite for this tile structure. This is used to display the structure in UI.
        /// </summary>
        /// <returns></returns>
        public Sprite GetIcon()
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, SpriteData.IconIndex);
        }
        
        /// <summary>
        ///     Returns sprite for this object using the provided rotation.
        /// </summary>
        /// <param name="_rotation"></param>
        /// <returns></returns>
        public Sprite GetSprite(ObjectRotation _rotation)
        {
            return SpriteCache.GetSprite(SpriteData.SpriteGroup, Rotatable ? (int)_rotation : 0);
        }

        public virtual bool CanPlace(Tile _tile)
        {
            return !_tile.HasObject;
        }
    }
}
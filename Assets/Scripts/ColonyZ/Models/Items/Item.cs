using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Models.Items
{
    public abstract class Item : ScriptableObject
    {
        public SpriteData ItemSpriteData => itemSpriteData;

        public int SpriteIndex => itemSpriteIndex;

        public string ItemName => itemName;

        public int MaxStackSize => maxStackSize;

        public int MaxDurability => maxDurability;

        public int Durability { get; protected set; }

        #region Serialized Fields

        [SerializeField] private SpriteData itemSpriteData;

        [SerializeField] private int itemSpriteIndex;

        [SerializeField] private string itemName;

        [SerializeField] private int maxStackSize;

        [SerializeField] private int maxDurability;

        #endregion
    }
}
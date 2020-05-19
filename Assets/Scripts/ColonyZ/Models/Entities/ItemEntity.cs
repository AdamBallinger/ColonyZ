using ColonyZ.Models.Items;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Sprites;
using UnityEngine;

namespace ColonyZ.Models.Entities
{
    public class ItemEntity : Entity
    {
        public ItemEntity(Tile _tile, ItemStack _itemStack) : base(_tile)
        {
            ItemStack = _itemStack;
            Name = ItemStack.Item.ItemName;
        }

        public ItemStack ItemStack { get; }

        public override void Update()
        {
            if (ItemStack.Quantity <= 0) World.Instance.RemoveItem(this);
        }

        public override int GetSortingOrder()
        {
            return base.GetSortingOrder() + 1;
        }

        public override Sprite GetSelectionIcon()
        {
            return SpriteCache.GetSprite(ItemStack.Item);
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() +
                   $"Quantity: {ItemStack.Quantity}\n";
        }
    }
}
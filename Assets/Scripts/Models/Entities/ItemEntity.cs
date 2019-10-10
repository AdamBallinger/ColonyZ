using Models.Items;
using Models.Map;
using Models.Map.Tiles;
using Models.Sprites;
using UnityEngine;

namespace Models.Entities
{
    public class ItemEntity : Entity
    {
        public ItemStack ItemStack { get; }
        
        public ItemEntity(Tile _tile, ItemStack _itemStack) : base(_tile)
        {
            ItemStack = _itemStack;
            Name = ItemStack.Item.ItemName;
        }

        public override void Update()
        {
            if (ItemStack.Quantity <= 0)
            {
                World.Instance.RemoveItem(this);
            }
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
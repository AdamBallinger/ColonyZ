using Models.Items;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Entities
{
    public class ItemEntity : Entity
    {
        public ItemStack ItemStack { get; private set; }
        
        public ItemEntity(Tile _tile, ItemStack _itemStack) : base(_tile)
        {
            ItemStack = _itemStack;
            Name = ItemStack.Item.ItemName;
        }

        public override void Update()
        {
            if (ItemStack.Quantity <= 0)
            {
                // Delete the item entity from the world.
            }
        }

        public override int GetSortingOrder()
        {
            return base.GetSortingOrder() + 1;
        }

        public override Sprite GetSelectionIcon()
        {
            return null;
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() +
                   $"Quantity: {ItemStack.Quantity}\n";
        }
    }
}
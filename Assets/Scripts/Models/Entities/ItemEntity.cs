using Models.Items;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Entities
{
    public class ItemEntity : Entity
    {
        private ItemStack itemStack;
        
        public ItemEntity(Tile _tile, ItemStack _itemStack) : base(_tile)
        {
            itemStack = _itemStack;
            Name = itemStack.Item.ItemName;
        }

        public override void Update()
        {
            if (itemStack.Quantity <= 0)
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
                   $"Quantity: {itemStack.Quantity}\n";
        }
    }
}
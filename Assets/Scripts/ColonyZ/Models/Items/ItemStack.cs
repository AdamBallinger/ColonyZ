using System;

namespace ColonyZ.Models.Items
{
    public class ItemStack
    {
        public Item Item { get; }

        public int Quantity { get; set; }

        public ItemStack(Item _item, int _quantity)
        {
            Item = _item;
            Quantity = Math.Min(_quantity, Item.MaxStackSize);
        }

        /// <summary>
        ///     Takes a given amount from this item stack and returns a new stack with that amount.
        /// </summary>
        /// <param name="_quantity"></param>
        /// <returns></returns>
        public ItemStack Take(int _quantity)
        {
            // If the requested quantity is more than the current stack quantity return null.
            if (_quantity > Quantity)
            {
                return null;
            }

            Quantity -= _quantity;

            return new ItemStack(ItemManager.CreateItem(Item.ItemName), _quantity);
        }

        /// <summary>
        ///     Try to merge the provided item stack with this stack.
        /// </summary>
        /// <param name="_other"></param>
        /// <returns></returns>
        public void Merge(ItemStack _other)
        {
            // Item stacks do not match.
            if (!Item.ItemName.Equals(_other.Item.ItemName))
            {
                return;
            }

            // Current stack can't hold any more items.
            if (Quantity == Item.MaxStackSize)
            {
                return;
            }

            // The given stack will fit without exceeding the stacks item max.
            if (Quantity + _other.Quantity <= Item.MaxStackSize)
            {
                Quantity += _other.Quantity;
                return;
            }

            var overfill = Quantity + _other.Quantity - Item.MaxStackSize;
            var accepted = _other.Quantity - overfill;
            Quantity += accepted;
            _other.Quantity -= accepted;
        }
    }
}
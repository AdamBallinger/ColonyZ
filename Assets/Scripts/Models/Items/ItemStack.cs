namespace Models.Items
{
    public class ItemStack
    {
        public Item Item { get; }
        
        public int Quantity { get; set; }

        public ItemStack(Item _item, int _quantity)
        {
            Item = _item;
            Quantity = _quantity;
        }
    }
}
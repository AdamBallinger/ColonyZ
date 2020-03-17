using Models.Items;

namespace Models.Inventory
{
    public interface IInventory
    {
        ItemStack GetItemStack();
    }
}
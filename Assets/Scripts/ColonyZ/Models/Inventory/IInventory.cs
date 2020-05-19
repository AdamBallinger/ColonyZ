using ColonyZ.Models.Items;

namespace ColonyZ.Models.Inventory
{
    public interface IInventory
    {
        ItemStack GetItemStack();
    }
}
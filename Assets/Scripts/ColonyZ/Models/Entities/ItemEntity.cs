using ColonyZ.Models.Items;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Saving;
using Newtonsoft.Json.Linq;

namespace ColonyZ.Models.Entities
{
    public class ItemEntity : Entity
    {
        public ItemStack ItemStack { get; private set; }

        public ItemEntity(Tile _tile, ItemStack _itemStack) : base(_tile)
        {
            ItemStack = _itemStack;
            Name = ItemStack.Item.ItemName;
        }

        /// <summary>
        ///     Used for loading only.
        /// </summary>
        public ItemEntity() : base(null)
        {
        }

        public override void Update()
        {
            if (ItemStack.Quantity <= 0) World.Instance.RemoveItem(this);
        }

        public override int GetSortingOrder()
        {
            return base.GetSortingOrder() + 1;
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() +
                   $"Quantity: {ItemStack.Quantity}\n";
        }

        public override void OnSave(SaveGameWriter _writer)
        {
            base.OnSave(_writer);
            _writer.WriteProperty("item_quantity", ItemStack.Quantity);
        }

        public override void OnLoad(JToken _dataToken)
        {
            base.OnLoad(_dataToken);
            var item = ItemManager.CreateItem(_dataToken["entity_name"].Value<string>());
            var quantity = _dataToken["item_quantity"].Value<int>();

            World.Instance.SpawnItem(item, quantity, CurrentTile);
        }
    }
}
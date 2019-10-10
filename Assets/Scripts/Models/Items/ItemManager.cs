using System.Collections.Generic;

namespace Models.Items
{
    public static class ItemManager
    {
        private static List<Item> items = new List<Item>();
        
        public static void RegisterItem(Item _item)
        {
            if (items.Contains(_item)) return;
            
            items.Add(_item);
        }

        /// <summary>
        /// Creates a new item instance of the given item name.
        /// </summary>
        /// <param name="_itemName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Item CreateItem<T>(string _itemName) where T : Item
        {
            foreach (var item in items)
            {
                if (item.ItemName.Equals(_itemName))
                {
                    return UnityEngine.Object.Instantiate(item) as T;
                }
            }

            return null;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace ColonyZ.Models.Items
{
    public static class ItemManager
    {
        public static List<Item> Items { get; } = new List<Item>();

        public static void RegisterItem(Item _item)
        {
            if (Items.Contains(_item)) return;

            Items.Add(_item);
        }

        /// <summary>
        ///     Creates a new item instance of the given item name.
        /// </summary>
        /// <param name="_itemName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateItem<T>(string _itemName) where T : Item
        {
            foreach (var item in Items)
                if (item.ItemName.Equals(_itemName))
                    return Object.Instantiate(item) as T;

            return null;
        }

        /// <summary>
        ///     Creates a new item instance of the given item name.
        /// </summary>
        /// <param name="_itemName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Item CreateItem(string _itemName)
        {
            return CreateItem<Item>(_itemName);
        }
    }
}
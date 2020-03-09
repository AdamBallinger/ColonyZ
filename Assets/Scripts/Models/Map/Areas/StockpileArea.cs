using Models.Items;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Areas
{
    public class StockpileArea : Area
    {
        private Tile[] tiles;
        
        public StockpileArea(int _x, int _y, int _width, int _height) : base(_x, _y, _width, _height)
        {
            AreaName = "Stockpile"; // TODO: Have an ID system for stockpiles and append to name.
            RequiresRoom = false;
            MinimumSize = new Vector2(2, 2);
            tiles = new Tile[_width * _height];
        }
        
        /// <summary>
        /// Returns whether the stockpile has a given item (and optional quantity) in its inventory.
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_quantity"></param>
        /// <returns></returns>
        public bool HasItem(Item _item, int _quantity = 1)
        {
            foreach (var tile in tiles)
            {
                if (tile.GetItem() != null && tile.GetItem().Item.ItemName.Equals(_item.ItemName) && 
                    tile.GetItem().Quantity >= _quantity)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
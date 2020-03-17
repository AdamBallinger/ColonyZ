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
        /// Returns the tile containing the given item and quantity inside the stockpile.
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_quantity"></param>
        /// <returns></returns>
        public Tile GetTileWithItem(Item _item, int _quantity)
        {
            // TODO: Prioritize tiles with lowest stack quantity first. Maybe use priority list with dictionary?
            foreach (var tile in tiles)
            {
                if (tile.GetItemStack() != null && tile.GetItemStack().Item.ItemName.Equals(_item.ItemName) &&
                    tile.GetItemStack().Quantity >= _quantity)
                {
                    return tile;
                }
            }

            return null;
        }
    }
}
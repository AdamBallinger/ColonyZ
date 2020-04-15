using Models.Items;
using Models.Map.Tiles;
using UnityEngine;

namespace Models.Map.Zones
{
    public class StorageZone : Zone
    {
        private Tile[] tiles;

        public StorageZone()
        {
            ZoneName = "Storage";
            CanContainObjects = false;
            MinimumSize = new Vector2(3, 2);
        }

        public override void SetSize(int _width, int _height)
        {
            base.SetSize(_width, _height);
            tiles = new Tile[_width * _height];
        }

        /// <summary>
        /// Returns the tile containing the given item and quantity inside the zone.
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
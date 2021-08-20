using ColonyZ.Models.Items;
using ColonyZ.Models.Map.Tiles;
using UnityEngine;

namespace ColonyZ.Models.Map.Zones
{
    public class StorageZone : Zone
    {
        public StorageZone()
        {
            ZoneName = "Storage " + (ZoneManager.Instance.GetZoneTypeQuantity<StorageZone>() + 1);
            CanContainObjects = false;
            Color = new Color(
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                0.05f);
        }

        /// <summary>
        ///     Returns the tile containing the given item and quantity inside the zone.
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_quantity"></param>
        /// <returns></returns>
        public Tile GetTileWithItem(Item _item, int _quantity)
        {
            // TODO: Prioritize tiles with lowest stack quantity first. Maybe use priority list with dictionary?
            foreach (var tile in Tiles)
                if (tile.GetItemStack() != null && tile.GetItemStack().Item.ItemName.Equals(_item.ItemName) &&
                    tile.GetItemStack().Quantity >= _quantity)
                    return tile;

            return null;
        }

        public override string GetSelectionName()
        {
            return ZoneName;
        }

        public override string GetSelectionDescription()
        {
            return $"Size: {Size}";
        }
    }
}
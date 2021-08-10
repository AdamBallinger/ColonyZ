using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Map.Areas
{
    public class Area
    {
        /// <summary>
        ///     Unique ID for this area.
        /// </summary>
        public int AreaID => AreaManager.Instance.GetAreaID(this);

        /// <summary>
        ///     Number of tiles inside the area.
        /// </summary>
        public int Size => Tiles.Count;

        /// <summary>
        ///     List of tiles that are assigned to this area.
        /// </summary>
        private HashSet<Tile> Tiles { get; }

        public Area()
        {
            Tiles = new HashSet<Tile>();
        }

        public void AssignTile(Tile _tile)
        {
            _tile.Area?.UnassignTile(_tile);
            Tiles.Add(_tile);
            _tile.Area = this;
            _tile.Region.Area = this;
        }

        /// <summary>
        ///     Removes the tile from the area.
        /// </summary>
        /// <param name="_tile"></param>
        public void UnassignTile(Tile _tile)
        {
            Tiles.Remove(_tile);
            _tile.Area = null;

            if (Tiles.Count == 0)
            {
                AreaManager.Instance.Areas.Remove(this);
            }
        }

        public void ReleaseTo(Area _area)
        {
            var list = Tiles.ToList();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                _area.AssignTile(list[i]);
            }
        }
    }
}
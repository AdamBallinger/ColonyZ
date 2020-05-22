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
        ///     List of tiles that assigned to this area.
        /// </summary>
        public HashSet<Tile> Tiles { get; }

        public HashSet<Area> LinkedAreas { get; }

        public Area()
        {
            Tiles = new HashSet<Tile>();
            LinkedAreas = new HashSet<Area>();
        }

        public void AssignTile(Tile _tile)
        {
            // Remove tile from its previous area.
            _tile.Area?.UnassignTile(_tile);

            Tiles.Add(_tile);
            _tile.Area = this;
        }

        /// <summary>
        ///     Removes the tile from the area.
        /// </summary>
        /// <param name="_tile"></param>
        public void UnassignTile(Tile _tile)
        {
            Tiles.Remove(_tile);
            _tile.Area = null;
        }

        /// <summary>
        ///     Unassign all tiles in the area.
        /// </summary>
        public void ReleaseTiles()
        {
            var tiles = Tiles.ToList();
            for (var i = Tiles.Count - 1; i >= 0; i--) tiles[i].Area?.UnassignTile(tiles[i]);
        }

        public void AddConnection(Area _area)
        {
            if (_area == null || LinkedAreas.Contains(_area)) return;

            LinkedAreas.Add(_area);
        }

        public bool HasConnection(Area _area)
        {
            return LinkedAreas.Contains(_area);
        }
    }
}
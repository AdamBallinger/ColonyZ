using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Regions;

namespace ColonyZ.Models.Map.Areas
{
    public class Area
    {
        /// <summary>
        ///     Returns number of tiles in this Area.
        /// </summary>
        public int SizeInTiles => Regions.Sum(r => r.Tiles.Count);

        public int AreaID => AreaManager.Instance.Areas.IndexOf(this) + 1;
        
        /// <summary>
        ///     List of areas that can be accessed from this area.
        /// </summary>
        public List<Area> LinkedAreas { get; }
        
        private List<Region> Regions { get; }

        public Area()
        {
            Regions = new List<Region>();
            LinkedAreas = new List<Area>();
        }

        /// <summary>
        ///     Adds a region to this area.
        /// </summary>
        /// <param name="_region"></param>
        public void AssignRegion(Region _region)
        {
            _region.Area?.UnassignRegion(_region);
            Regions.Add(_region);
            _region.Area = this;
        }

        /// <summary>
        ///     Removes the region from the area.
        /// </summary>
        /// <param name="_region"></param>
        public void UnassignRegion(Region _region)
        {
            Regions.Remove(_region);
            _region.Area = null;
        
            if (Regions.Count == 0)
            {
                AreaManager.Instance.Areas.Remove(this);
            }
        }

        public void ReleaseTo(Area _area)
        {
            for (var i = Regions.Count - 1; i >= 0; i--)
            {
                _area.AssignRegion(Regions[i]);
            }
        }

        public void AddConnection(Area _area)
        {
            if (LinkedAreas.Contains(_area)) return;
            LinkedAreas.Add(_area);
        }
    }
}
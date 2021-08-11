using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Map.Regions;

namespace ColonyZ.Models.Map.Areas
{
    public class Area
    {
        public List<Region> Regions { get; }

        /// <summary>
        ///     Returns number of tiles in this Area.
        /// </summary>
        public int SizeInTiles => Regions.Sum(r => r.Tiles.Count);

        public int SizeInRegions => Regions.Count;

        public int AreaID => AreaManager.Instance.Areas.IndexOf(this) + 1;

        public Area()
        {
            Regions = new List<Region>();
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
    }
}
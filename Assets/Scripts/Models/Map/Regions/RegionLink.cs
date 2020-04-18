using UnityEngine;

namespace Models.Map.Regions
{
    public class RegionLink
    {
        public EdgeSpan Span { get; set; }

        private Region[] regions = new Region[2];

        /// <summary>
        /// Returns the region linked to the given region by this link.
        /// </summary>
        /// <param name="_region"></param>
        /// <returns></returns>
        public Region GetOther(Region _region)
        {
            if (regions[0] != _region && regions[1] != _region) return null;

            return regions[0] != _region ? regions[0] : regions[1];
        }

        public void Assign(Region _region)
        {
            if (regions[0] == _region || regions[1] == _region)
            {
                Debug.LogError("Trying to double assign region to region link.");
                return;
            }

            if (regions[0] == null)
            {
                regions[0] = _region;
            }
            else if (regions[1] == null)
            {
                regions[1] = _region;
            }
            else
            {
                Debug.LogError("Can't assign more than 2 regions to a region link.");
            }
        }

        public void Unassign(Region _region)
        {
            if (regions[0] != _region && regions[1] != _region)
            {
                Debug.LogError("Trying to unassign region from a link it was never assigned to.");
                return;
            }

            if (regions[0] == _region)
            {
                regions[0] = null;
            }
            else if (regions[1] == _region)
            {
                regions[1] = null;
            }

            if (regions[0] == null && regions[1] == null)
            {
                RegionLinksDatabase.NotifyEmptyLink(this);
            }
        }
    }
}
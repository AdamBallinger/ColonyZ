using System.Collections.Generic;

namespace Models.Map.Regions
{
    public static class RegionReachabilityChecker
    {
        /// <summary>
        /// Determines if a source region can access a target region through its region links.
        /// This does not take into consideration traversal permissions for a region.
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        public static bool CanReachRegion(Region _source, Region _target)
        {
            if (_source == null || _target == null) return false;
            if (_source == _target) return true;

            var queue = new Queue<Region>();
            var visited = new HashSet<Region>();
            queue.Enqueue(_source);
            visited.Add(_source);

            while (queue.Count > 0)
            {
                var region = queue.Dequeue();

                if (region == _target)
                {
                    return true;
                }

                foreach (var link in region.Links)
                {
                    var other = link.GetOther(region);
                    if (other == null) continue;
                    if (visited.Contains(other)) continue;

                    queue.Enqueue(other);
                    visited.Add(other);
                }
            }

            return false;
        }
    }
}
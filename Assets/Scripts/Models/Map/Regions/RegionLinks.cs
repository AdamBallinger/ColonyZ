using System.Collections.Generic;

namespace Models.Map.Regions
{
    public class RegionLinks
    {
        public static Dictionary<int, List<Region>> Links { get; private set; }
            = new Dictionary<int, List<Region>>();

        public static void Add(Region _region)
        {
            var links = GetRegionHashes(_region);

            foreach (var hash in links)
            {
                if (!Links.ContainsKey(hash))
                {
                    Links.Add(hash, new List<Region>());
                }
                else
                {
                    if (!Links[hash].Contains(_region))
                    {
                        Links[hash].Add(_region);
                    }
                }
            }
        }

        public static void Remove(Region _region)
        {
            var links = GetRegionHashes(_region);

            foreach (var hash in links)
            {
                if (Links.ContainsKey(hash))
                {
                    if (Links[hash].Contains(_region))
                    {
                        Links[hash].Remove(_region);

                        if (Links[hash].Count == 0)
                        {
                            Links.Remove(hash);
                        }
                    }
                }
            }
        }

        public static List<Region> GetRegionNeighbours(Region _region)
        {
            var hashes = GetRegionHashes(_region);

            var regions = new List<Region>();

            foreach (var hash in hashes)
            {
                if (Links.ContainsKey(hash))
                {
                    foreach (var region in Links[hash])
                    {
                        if (region != _region)
                        {
                            regions.Add(region);
                        }
                    }
                }
            }

            return regions;
        }

        /// <summary>
        /// Calculates the hash for region links. Links are calculated going right and up.
        /// </summary>
        /// <param name="_region"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetRegionHashes(Region _region)
        {
            var hashes = new List<int>();


            return hashes;
        }
    }
}
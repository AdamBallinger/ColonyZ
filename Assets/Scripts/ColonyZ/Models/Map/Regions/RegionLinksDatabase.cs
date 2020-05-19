using System.Collections.Generic;

namespace ColonyZ.Models.Map.Regions
{
    public static class RegionLinksDatabase
    {
        private static Dictionary<ulong, RegionLink> Links { get; } = new Dictionary<ulong, RegionLink>();

        public static RegionLink LinkFromSpan(EdgeSpan _span)
        {
            var code = _span.UniqueHashCode();

            if (!Links.TryGetValue(code, out var link))
            {
                link = new RegionLink();
                link.Span = _span;
                Links.Add(code, link);
            }

            return link;
        }

        public static void NotifyEmptyLink(RegionLink _link)
        {
            Links.Remove(_link.Span.UniqueHashCode());
        }
    }
}
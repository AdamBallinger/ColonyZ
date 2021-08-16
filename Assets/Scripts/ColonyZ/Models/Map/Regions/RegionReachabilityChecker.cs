namespace ColonyZ.Models.Map.Regions
{
    public static class RegionReachabilityChecker
    {
        /// <summary>
        ///     Determines if a source region can access a target region.
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_target"></param>
        /// <returns></returns>
        public static bool CanReachRegion(Region _source, Region _target)
        {
            if (_source == null || _target == null) return false;
            if (_source.Area == null || _target.Area == null) return false;
            
            return _source.Area.AreaID == _target.Area.AreaID || _source.Area.LinkedAreas.Contains(_target.Area);
        }
    }
}
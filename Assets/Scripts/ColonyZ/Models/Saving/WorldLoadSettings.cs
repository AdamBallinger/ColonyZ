using ColonyZ.Models.Map;

namespace ColonyZ.Models.Saving
{
    public enum WorldLoadType
    {
        New,
        Load
    }

    public static class WorldLoadSettings
    {
        public static WorldLoadType LOAD_TYPE = WorldLoadType.New;
        public static WorldSizeTypes.WorldSize WORLD_SIZE;

        public static bool FROM_MENU = false;
    }
}
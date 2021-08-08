namespace ColonyZ.Models.Map.Tiles.Objects.Factory
{
    public static class ObjectFactories
    {
        public static readonly TileObjectFactory WallFactory = new WallObjectFactory();
        public static readonly TileObjectFactory DoorFactory = new DoorObjectFactory();
        public static readonly TileObjectFactory ResourceFactory = new ResourceObjectFactory();
    }
}
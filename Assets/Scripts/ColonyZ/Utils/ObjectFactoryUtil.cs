using ColonyZ.Models.Map.Tiles.Objects.Factory;

namespace ColonyZ.Utils
{
    public static class ObjectFactoryUtil
    {
        /// <summary>
        ///     Returns the object factory relevant to the given factory type.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static TileObjectFactory GetFactory(ObjectFactoryType _type)
        {
            switch (_type)
            {
                case ObjectFactoryType.Wall:
                    return ObjectFactories.WallFactory;
                case ObjectFactoryType.Door:
                    return ObjectFactories.DoorFactory;
                case ObjectFactoryType.Furniture:
                    return ObjectFactories.FurnitureFactory;
                case ObjectFactoryType.Resource:
                    return ObjectFactories.ResourceFactory;
            }

            return null;
        }
    }
}
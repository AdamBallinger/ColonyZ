using Models.Entities.Living;
using Models.Map.Tiles;

namespace Models.Map.Pathing
{
    public static class PathingUtils
    {
        /// <summary>
        /// Checks if a given entity can path to a given tile.
        /// This method will not calculate a path asynchronously, and should not be called excessively.
        /// </summary>
        /// <param name="_entity"></param>
        /// <param name="_tile"></param>
        /// <returns></returns>
       public static bool CanEntityReachTile(LivingEntity _entity, Tile _tile)
       {
           return PathFinder.TestPath(_entity.CurrentTile, _tile);
       }
    }
}
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;

namespace ColonyZ.Models.AI.Jobs
{
    public class BuildJob : Job
    {
        private TileObject tileObject;

        public BuildJob(Tile _targetTile, TileObject _object) : base(_targetTile)
        {
            JobName = "Build: " + _object.ObjectName;
            tileObject = _object;
        }

        public override void OnComplete()
        {
            base.OnComplete();

            TargetTile.SetObject(tileObject);
        }
    }
}
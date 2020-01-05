using Models.Map.Tiles;
using Models.Map.Tiles.Objects;

namespace Models.AI.Jobs
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
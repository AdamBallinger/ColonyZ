using Models.Map.Tiles;
using Models.Map.Tiles.Objects;
using Models.TimeSystem;

namespace Models.Jobs
{
    public class BuildJob : Job
    {
        private TileObject tileObject;
        
        public BuildJob(Tile _targetTile, TileObject _object) : base(_targetTile)
        {
            JobName = "Build: " + _object.ObjectName;
            tileObject = _object;
        }

        public override void Update()
        {
            if (Progress >= 1.0f) return;

            if (AssignedEntity == null) return;

            if (AssignedEntity.CurrentTile == TargetTile)
            {
                Progress += TimeManager.Instance.DeltaTime;
            }
        }

        public override void OnComplete()
        {
            base.OnComplete();
            
            TargetTile.SetObject(tileObject);
        }
    }
}
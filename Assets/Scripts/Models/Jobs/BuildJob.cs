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
            tileObject = _object;
            // TODO: Logically set working tile for the job. Check surrounding tiles for a free tile.
        }

        public override void Update()
        {
            if (Progress >= 1.0f) return;

            if (AssignedEntity == null) return;
            
            if (AssignedEntity.CurrentTile != WorkingTile && !AssignedEntity.Motor.Working)
            {
                AssignedEntity.Motor.SetTargetTile(WorkingTile);
            }
            
            if (AssignedEntity.CurrentTile == WorkingTile)
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
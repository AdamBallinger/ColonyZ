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
        }

        public override void Update()
        {
            if (Progress >= 1.0f) return;
            
            Progress += TimeManager.Instance.DeltaTime;
            
            // TODO: Change to an OnComplete event.
            if (Progress >= 1.0f)
            {
                TargetTile.SetObject(tileObject);
            }
        }
    }
}
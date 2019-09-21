using Models.Jobs;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Tiles;

namespace Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public Job CurrentJob { get; private set; }
        
        public HumanEntity(Tile _tile) : base(_tile)
        {
            
        }
        
        public bool SetJob(Job _job, bool _forceStop = false)
        {
            if (!_forceStop && CurrentJob != null && CurrentJob.Progress < 1.0f) return false;

            CurrentJob = _job;
            Motor.Stop();
            return true;
        }

        public override void Update()
        {
            base.Update();
            
            if (!Motor.Working && CurrentJob == null)
            {
                Motor.SetTargetTile(World.Instance.GetRandomTile());
            }

            // TODO: Should maybe use action system for handling jobs?
            if (CurrentJob == null) return;
            
            if (!Motor.Working)
            {
                Motor.SetTargetTile(CurrentJob.WorkingTile);
            }
            
            // TODO: Fix this if the working tile is enterable but not pathable.
            if (CurrentJob.WorkingTile.GetEnterability() != TileEnterability.Immediate)
            {
                var newTileFound = false;
                // TODO: If the only pathable tile has an active job, set the job for that tile as invalid.
                foreach (var tile in CurrentJob.TargetTile.DirectNeighbours)
                {
                    if (PathFinder.TestPath(CurrentTile, tile))
                    {
                        CurrentJob.WorkingTile = tile;
                        Motor.SetTargetTile(CurrentJob.WorkingTile);
                        newTileFound = true;
                        break;
                    }
                }
                
                if (!newTileFound)
                {
                    JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
                    SetJob(null, true);
                    return;
                }
            }
            
            CurrentJob?.Update();
        }
    }
}

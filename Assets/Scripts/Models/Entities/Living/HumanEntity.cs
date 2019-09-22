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

            if (CurrentJob.WorkingTile.GetEnterability() != TileEnterability.Immediate)
            {
                var newTileFound = false;
                // TODO: If the only pathable tile has an active job, set the job for that tile as invalid.
                foreach (var tile in CurrentJob.TargetTile.DirectNeighbours)
                {
                    // Don't consider tiles that have a job, or are not enterable.
                    if (tile.CurrentJob != null || tile.GetEnterability() != TileEnterability.Immediate) continue;
                    
                    // TODO: Change this as it is very slow on large maps, especially with a lot of entities.
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
                    return;
                }
            }
            
            CurrentJob?.Update();
        }

        public override void OnPathFailed()
        {
            // If the last path request failed for the ai motor, then the entity can't reach the working tile for this job.
            if (CurrentJob != null)
            {
                JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
            }
        }
    }
}

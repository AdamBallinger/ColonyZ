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
                // Checks if the working tile can be reached before requesting a path.
                if (PathFinder.TestPath(CurrentTile, CurrentJob.WorkingTile))
                {
                    Motor.SetTargetTile(CurrentJob.WorkingTile);
                }
                else
                {
                    // Mark the job as invalid if the path can't be reached. TODO: Don't flag as invalid if other entity can reach job.
                    JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
                    return;
                }
            }

            if (CurrentJob.WorkingTile.GetEnterability() != TileEnterability.Immediate)
            {
                var newTileFound = false;
                // TODO: If the only pathable tile has an active job, set the job for that tile as invalid.
                foreach (var tile in CurrentJob.TargetTile.DirectNeighbours)
                {
                    if (tile.CurrentJob != null) continue;
                    
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
    }
}

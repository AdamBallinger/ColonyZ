using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public Job CurrentJob { get; private set; }

        public HumanEntity(Tile _tile) : base(_tile)
        {
        }

        public bool SetJob(Job _job, bool _forceStop = false)
        {
            if (!_forceStop && CurrentJob != null && CurrentJob.Complete) return false;

            CurrentJob = _job;
            Motor.Stop();
            return true;
        }

        public override void Update()
        {
            base.Update();

            if (!Motor.Working && CurrentJob == null)
            {
                Motor.SetTargetTile(World.Instance.GetRandomTileAround(CurrentTile, 16));
                return;
            }

            if (CurrentJob == null) return;

            // Check if the working tile for the job became unreachable due to another job being completed.
            // A working tile would have a null area if the tile has an object built on it.
            // if (CurrentJob.WorkingTile.Area == null
            //     || !CurrentJob.WorkingTile.Area.HasConnection(CurrentTile.Area))
            // {
            //     JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
            //     return;
            // }

            // Try find a new working tile if the current tile is no longer enter-able.
            if (CurrentJob.WorkingTile.GetEnterability() != TileEnterability.Immediate)
            {
                var closestTile =
                    JobManager.Instance.GetClosestEnterableNeighbour(this, CurrentJob.TargetTile.DirectNeighbours);

                if (closestTile != null)
                {
                    CurrentJob.WorkingTile = closestTile;
                    Motor.SetTargetTile(CurrentJob.WorkingTile);
                }
                else
                {
                    JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
                    return;
                }
            }

            // If a new closest tile is found for the job, switch to it.
            // TODO: Optimise this so that it only checks when the angle to the job changed?
            var closeTile =
                JobManager.Instance.GetClosestEnterableNeighbour(this, CurrentJob.TargetTile.DirectNeighbours);
            if (closeTile != null && closeTile != CurrentJob.WorkingTile)
            {
                CurrentJob.WorkingTile = closeTile;
                Motor.SetTargetTile(closeTile);
            }
            else if (closeTile == null)
            {
                JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
            }

            CurrentJob?.Update();
        }

        public override void OnPathFailed()
        {
            // If the last path request failed for the ai motor, then the entity can't reach the working tile for this job.
            if (CurrentJob != null) JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
        }
    }
}
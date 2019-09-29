﻿using Models.Jobs;
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
                Motor.SetTargetTile(World.Instance.GetRandomTile());
            }

            // TODO: Should maybe use action system for handling jobs?
            if (CurrentJob == null) return;

            // Try find a new working tile if the current tile is no longer enterable.
            if (CurrentJob.WorkingTile.GetEnterability() != TileEnterability.Immediate)
            {
                var closestTile = JobManager.Instance.GetClosestEnterableNeighbour(this, CurrentJob.TargetTile.DirectNeighbours);
                
                if (closestTile != null && PathFinder.TestPath(CurrentTile, closestTile))
                {
                    CurrentJob.WorkingTile = closestTile;
                    Motor.SetTargetTile(CurrentJob.WorkingTile);
                }
                else
                {
                    JobManager.Instance.NotifyActiveJobInvalid(CurrentJob);
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

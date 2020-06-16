using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Sprites;
using ColonyZ.Models.TimeSystem;

namespace ColonyZ.Models.Entities.Living
{
    public class HumanEntity : LivingEntity
    {
        public Job CurrentJob { get; private set; }

        public bool HasJob => CurrentJob != null;

        private Cardinals previousJobCardinal;

        /// <summary>
        /// Time in seconds between being able to check if a new working tile is available when cardinal direction changes.
        /// </summary>
        private const float CARDINAL_CHECK_TIMER = 2.0f;

        private float currentCardinalCheckTime = 0.0f;

        public HumanEntity(Tile _tile) : base(_tile)
        {
        }

        public bool SetJob(Job _job, bool _forceStop = false)
        {
            if (!_forceStop && HasJob && CurrentJob.Complete) return false;

            previousJobCardinal = GetCurrentJobCardinal();
            currentCardinalCheckTime = 0.0f;

            CurrentJob = _job;
            Motor.FinishPath();
            return true;
        }

        public override void Update()
        {
            base.Update();

            if (!Motor.Working && !HasJob)
            {
                Motor.SetTargetTile(World.Instance.GetRandomTileAround(CurrentTile, 16, true));
                return;
            }

            if (!HasJob) return;

            if (!Motor.Working ||
                CurrentJob.WorkingTile.GetEnterability() == TileEnterability.None)
            {
                RecalculateWorkingTile();
            }

            var currentJobCardinal = GetCurrentJobCardinal();
            if (currentJobCardinal != previousJobCardinal && currentCardinalCheckTime >= CARDINAL_CHECK_TIMER)
            {
                currentCardinalCheckTime = 0.0f;
                // If a new closest tile is found for the job, switch to it.
                RecalculateWorkingTile();
            }

            currentCardinalCheckTime += TimeManager.Instance.UnscaledDeltaTime;
            CurrentJob?.Update();
            previousJobCardinal = currentJobCardinal;
        }

        public override void OnPathFailed()
        {
            // If the last path request failed for the ai motor, then the entity can't reach the working tile for this job.
            if (HasJob) JobManager.Instance.NotifyWorkerCantAccessJob(CurrentJob);
        }

        private void RecalculateWorkingTile()
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
                JobManager.Instance.NotifyWorkerCantAccessJob(CurrentJob);
            }
        }

        private Cardinals GetCurrentJobCardinal()
        {
            if (!HasJob) return previousJobCardinal;

            var cX = CurrentTile.X;
            var cY = CurrentTile.Y;
            var jX = CurrentJob.TargetTile.X;
            var jY = CurrentJob.TargetTile.Y;

            if (cX == jX || cY == jY)
            {
                if (cX < jX) return Cardinals.West;
                if (cX > jX) return Cardinals.East;
                if (cY < jY) return Cardinals.South;
                if (cY > jY) return Cardinals.North;
            }
            else
            {
                if (cX > jX && cY > jY) return Cardinals.North_East;
                if (cX < jX && cY > jY) return Cardinals.North_West;
                if (cX > jX && cY < jY) return Cardinals.South_East;
                if (cX < jX && cY < jY) return Cardinals.South_West;
            }

            return previousJobCardinal;
        }

        public override string GetSelectionDescription()
        {
            return base.GetSelectionDescription() + $"Has Job: {HasJob}\n";
        }
    }
}
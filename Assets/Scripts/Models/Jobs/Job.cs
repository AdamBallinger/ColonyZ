using Models.Entities.Living;
using Models.Map.Tiles;
using Models.TimeSystem;

namespace Models.Jobs
{
    public abstract class Job
    {
        /// <summary>
        /// Name of the job that is displayed on the UI.
        /// </summary>
        public string JobName { get; protected set; }
        
        /// <summary>
        /// Current progress of the job.
        /// </summary>
        protected float Progress { get; set; }
        
        /// <summary>
        /// Time in seconds it takes to complete the job. Default: 1 second.
        /// </summary>
        public float WorkTime { get; protected set; }

        /// <summary>
        /// Checks if the job is completed.
        /// </summary>
        public bool Complete => Progress >= WorkTime;

        /// <summary>
        /// Reference to the entity assigned to complete this job.
        /// </summary>
        public HumanEntity AssignedEntity { get; set; }
        
        /// <summary>
        /// The tile the job is modifying.
        /// </summary>
        public Tile TargetTile { get; }
        
        /// <summary>
        /// The tile the entity will complete the job on. Defaults to the target tile.
        /// </summary>
        public Tile WorkingTile { get; set; }

        protected Job(Tile _targetTile)
        {
            TargetTile = _targetTile;
            WorkingTile = _targetTile;
            WorkTime = 1.0f;
        }

        public virtual void Update()
        {
            if (Complete) return;

            if (AssignedEntity == null) return;

            if (AssignedEntity.CurrentTile == WorkingTile && TargetTile.LivingEntities.Count == 0)
            {
                Progress += TimeManager.Instance.DeltaTime;
            }
        }
        
        public virtual void OnComplete()
        {
            AssignedEntity.SetJob(null, true);
            TargetTile.CurrentJob = null;
        }
    }
}
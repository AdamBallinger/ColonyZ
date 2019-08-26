using Models.Entities.Living;
using Models.Map.Tiles;

namespace Models.Jobs
{
    public abstract class Job
    {
        /// <summary>
        /// Name of the job as it appears. E.g "Build Job"
        /// </summary>
        public string JobName { get; }
        
        /// <summary>
        /// Current progress of the job. Value >= 1.0f is completed.
        /// </summary>
        public float Progress { get; protected set; }

        /// <summary>
        /// Reference to the entity assigned to complete this job.
        /// </summary>
        public HumanEntity AssignedEntity { get; set; }
        
        /// <summary>
        /// The tile the job is modifying.
        /// </summary>
        public Tile TargetTile { get; }
        
        /// <summary>
        /// The tile the entity will move to to work on the job.
        /// </summary>
        public Tile WorkingTile { get; set; }
        
        protected Job(Tile _targetTile)
        {
            TargetTile = _targetTile;
        }

        public abstract void Update();
    }
}
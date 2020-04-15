using System;
using System.Collections.Generic;
using System.Linq;
using Models.Entities;
using Models.Entities.Living;
using Models.Map;
using Models.Map.Tiles;
using Models.TimeSystem;

namespace Models.AI.Jobs
{
    public class JobManager
    {
        public static JobManager Instance { get; private set; }

        /// <summary>
        /// Total number of current jobs.
        /// </summary>
        public int JobCount => Jobs.Count;

        public int ActiveCount => Jobs.Count(j => j.State == JobState.Active);

        public int IdleCount => Jobs.Count(j => j.State == JobState.Idle);

        public int ErrorCount => Jobs.Count(j => j.State == JobState.Error);

        /// <summary>
        /// List of all created jobs.
        /// </summary>
        private List<Job> Jobs { get; set; }

        /// <summary>
        /// A map for each job that keeps track of which entities can't reach it.
        /// </summary>
        private Dictionary<Job, List<HumanEntity>> jobNoAccessMap { get; set; }

        /// <summary>
        /// Event called when a new job is added to the job manager.
        /// </summary>
        public event Action<Job> jobCreatedEvent;

        /// <summary>
        /// Event called when the status of a job changes. E.g. A job being marked as invalid and vice versa.
        /// </summary>
        public event Action<Job> jobStateChangedEvent;

        /// <summary>
        /// Event called when a job has been completed.
        /// </summary>
        public event Action<Job> jobCompletedEvent;

        /// <summary>
        /// Time in seconds between each automatic evaluation of errored jobs.
        /// </summary>
        private const float ERROR_JOB_CHECK_INTERVAL = 2.0f;

        /// <summary>
        /// Current timer for auto checking errored jobs.
        /// </summary>
        private float jobErrorTimer;

        private JobManager()
        {
        }

        public static void Create()
        {
            if (Instance == null)
            {
                Instance = new JobManager();
                Instance.Init();
            }
        }

        private void Init()
        {
            Jobs = new List<Job>();
            jobNoAccessMap = new Dictionary<Job, List<HumanEntity>>();
        }

        /// <summary>
        /// Event called when an entity has finished a job.
        /// </summary>
        /// <param name="_completedJob"></param>
        private void OnJobFinished(Job _completedJob)
        {
            Jobs.Remove(_completedJob);
            _completedJob.TargetTile.CurrentJob = null;
            _completedJob.OnComplete();
            jobCompletedEvent?.Invoke(_completedJob);
        }

        /// <summary>
        /// Checks if any job in the invalid jobs list is now completable.
        /// </summary>
        private void EvaluateInvalidJobs()
        {
            var entities = World.Instance.Characters;

            for (var i = Jobs.Count - 1; i >= 0; i--)
            {
                var isJobReachable = false;
                var job = Jobs[i];

                if (job.State != JobState.Error) continue;

                foreach (var livingEntity in entities)
                {
                    var humanEntity = livingEntity as HumanEntity;

                    if (CanEntityReachJob(humanEntity, job))
                    {
                        isJobReachable = true;

                        if (jobNoAccessMap[job].Contains(humanEntity))
                        {
                            jobNoAccessMap[job].Remove(humanEntity);
                        }
                    }
                }

                if (isJobReachable)
                {
                    SetJobState(job, JobState.Idle);
                }
            }
        }

        private void AssignEntityJob(HumanEntity _entity, Job _job)
        {
            // TODO: Check if job is completable before setting a job (resources etc.).
            if (_entity.SetJob(_job))
            {
                _job.AssignedEntity = _entity;
                _entity.Motor.SetTargetTile(_job.WorkingTile);
                SetJobState(_job, JobState.Active);
            }
        }

        public void Update()
        {
            jobErrorTimer += TimeManager.Instance.UnscaledDeltaTime;

            if (Jobs.Count == 0)
            {
                return;
            }

            if (jobErrorTimer >= ERROR_JOB_CHECK_INTERVAL)
            {
                jobErrorTimer = 0.0f;
                EvaluateInvalidJobs();
            }

            for (var i = Jobs.Count - 1; i >= 0; i--)
            {
                var job = Jobs[i];
                if (job.State == JobState.Active && job.Complete)
                {
                    jobNoAccessMap.Remove(job);
                    OnJobFinished(job);
                }
            }

            // TODO: Only get players entities in future.
            var entities = World.Instance.Characters;

            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i] as HumanEntity;

                // Only use entities that are available.
                if (entity?.CurrentJob != null) continue;

                var job = Jobs.FirstOrDefault(j => j.State == JobState.Idle);

                if (job == null) break;

                if (jobNoAccessMap[job].Contains(entity))
                {
                    continue;
                }

                var closestTile = GetClosestEnterableNeighbour(entity, job.TargetTile.DirectNeighbours);

                if (closestTile != null)
                {
                    job.WorkingTile = closestTile;
                    AssignEntityJob(entity, job);
                    break;
                }

                // Entity can't reach the job so add it to the jobs map.
                jobNoAccessMap[job].Add(entity);

                // If the last entity can't reach the job, then the job must be unreachable.
                if (i == entities.Count - 1)
                {
                    SetJobState(job, JobState.Error);
                }
            }
        }

        private void SetJobState(Job _job, JobState _state)
        {
            _job.State = _state;
            jobStateChangedEvent?.Invoke(_job);
        }

        private bool CanEntityReachJob(Entity _entity, Job _job)
        {
            if (_entity == null || _job == null) return false;

            var closestTile = GetClosestEnterableNeighbour(_entity, _job.TargetTile.DirectNeighbours);

            return closestTile != null;
        }

        private void AddJob(Job _job)
        {
            if (_job == null) return;

            // TODO: Move to mouse controller to visualise duplication for demolish jobs etc.
            if (_job.TargetTile.CurrentJob != null) return;

            jobNoAccessMap.Add(_job, new List<HumanEntity>());

            Jobs.Add(_job);
            _job.TargetTile.CurrentJob = _job;
            _job.State = JobState.Idle;
            jobCreatedEvent?.Invoke(_job);
            jobStateChangedEvent?.Invoke(_job);
        }

        public void AddJobs(IEnumerable<Job> _jobs)
        {
            foreach (var job in _jobs)
            {
                AddJob(job);
            }
        }

        /// <summary>
        /// Notifies the job manager that an active job has become invalid.
        /// </summary>
        /// <param name="_job"></param>
        public void NotifyActiveJobInvalid(Job _job)
        {
            jobNoAccessMap[_job].Add(_job.AssignedEntity);
            _job.AssignedEntity.SetJob(null, true);

            SetJobState(_job, JobState.Error);
        }

        public Tile GetClosestEnterableNeighbour(Entity _entity, IReadOnlyCollection<Tile> _tiles)
        {
            if (_entity == null || _tiles == null || _tiles.Count <= 0) return null;

            Tile closest = null;
            var closestDist = float.MaxValue;

            var entityTile = _entity.CurrentTile;

            foreach (var tile in _tiles)
            {
                if (tile.GetEnterability() != TileEnterability.Immediate) continue;
                if (entityTile.Area == null) continue;
                // Skip the tile if entities current room has no connection to the tiles room.
                if (!entityTile.Area.HasConnection(tile.Area)) continue;

                var dist = (entityTile.Position - tile.Position).sqrMagnitude;

                // Make tiles that have a job assigned appear more expensive.
                if (tile.CurrentJob != null) dist += 1000.0f;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = tile;
                }
            }

            return closest;
        }
    }
}
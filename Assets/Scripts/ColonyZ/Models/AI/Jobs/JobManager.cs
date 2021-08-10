using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Entities;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;

namespace ColonyZ.Models.AI.Jobs
{
    public class JobManager
    {
        public static JobManager Instance { get; private set; }

        /// <summary>
        ///     Flag to determine if the manager is allowed to evaluate jobs in error state.
        /// </summary>
        private bool canEvaluateErrored = true;

        /// <summary>
        ///     List of chunks required to be able to evaluate error state jobs again.
        /// </summary>
        private List<WorldChunk> requiredChunks;

        /// <summary>
        ///     Total number of current jobs.
        /// </summary>
        public int JobCount => Jobs.Count;

        public int ActiveCount => Jobs.Count(j => j.State == JobState.Active);

        public int IdleCount => Jobs.Count(j => j.State == JobState.Idle);

        public int ErrorCount => Jobs.Count(j => j.State == JobState.Error);

        /// <summary>
        ///     List of all created jobs.
        /// </summary>
        public List<Job> Jobs { get; private set; }

        /// <summary>
        ///     A map for each job that keeps track of which entities can't reach it.
        /// </summary>
        private Dictionary<Job, List<HumanEntity>> jobNoAccessMap { get; set; }

        /// <summary>
        ///     Event called when a new job is added to the job manager.
        /// </summary>
        public event Action<Job> jobCreatedEvent;

        /// <summary>
        ///     Event called when the status of a job changes. E.g. A job being marked as invalid and vice versa.
        /// </summary>
        public event Action<Job> jobStateChangedEvent;

        /// <summary>
        ///     Event called when a job has been completed.
        /// </summary>
        public event Action<Job> jobCompletedEvent;

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

        public static void Destroy()
        {
            World.Instance.WorldGrid.chunkModifiedEvent -= Instance.OnChunkModified;
            World.Instance.onEntityRemoved -= Instance.OnEntityRemoved;
            Instance = null;
        }

        private void Init()
        {
            Jobs = new List<Job>();
            jobNoAccessMap = new Dictionary<Job, List<HumanEntity>>();
            requiredChunks = new List<WorldChunk>();

            World.Instance.WorldGrid.chunkModifiedEvent += OnChunkModified;
            World.Instance.onEntityRemoved += OnEntityRemoved;
        }

        private void OnChunkModified(WorldChunk _chunk)
        {
            // TODO: Broke, as it means it only checks again if the chunk the jobs are in changed..
            //if (requiredChunks.Contains(_chunk))
            {
                canEvaluateErrored = true;
            }
        }

        private void OnEntityRemoved(Entity _entity)
        {
            if (!(_entity is HumanEntity)) return;

            var human = (HumanEntity) _entity;
            if (human.HasJob)
            {
                var job = human.CurrentJob;
                job.AssignedEntity = null;
                SetJobState(job, JobState.Idle);
            }
        }

        /// <summary>
        ///     Event called when an entity has finished a job.
        /// </summary>
        /// <param name="_completedJob"></param>
        private void OnJobFinished(Job _completedJob)
        {
            Jobs.Remove(_completedJob);
            _completedJob.TargetTile.CurrentJob = null;
            _completedJob.OnComplete();
            EvaluateErrorJobs();
            jobCompletedEvent?.Invoke(_completedJob);
        }

        /// <summary>
        ///     Checks all jobs with an error state if they can now be completed.
        /// </summary>
        private void EvaluateErrorJobs()
        {
            if (!canEvaluateErrored) return;

            requiredChunks.Clear();

            var entities = World.Instance.Characters;

            var anyChanged = false;

            foreach (var job in Jobs)
            {
                if (job.State != JobState.Error) continue;

                var isJobReachable = false;

                foreach (var livingEntity in entities)
                {
                    var humanEntity = livingEntity as HumanEntity;
                    if (humanEntity == null) continue;

                    if (CanEntityReachJob(humanEntity, job))
                    {
                        isJobReachable = true;

                        if (jobNoAccessMap[job].Contains(humanEntity))
                        {
                            jobNoAccessMap[job].Remove(humanEntity);
                            if (!humanEntity.HasJob)
                            {
                                AssignEntityJob(humanEntity, job);
                                break;
                            }
                        }
                    }

                    // Add all chunks around the target tile to the required list if it
                    // can't be reached.
                    foreach (var tile in job.TargetTile.DirectNeighbours)
                    {
                        var chunk = World.Instance.WorldGrid.GetChunkAt(tile);
                        if (requiredChunks.Contains(chunk)) continue;
                        requiredChunks.Add(chunk);
                    }
                }

                if (isJobReachable && job.State != JobState.Active)
                {
                    anyChanged = true;
                    SetJobState(job, JobState.Idle);
                }
            }

            if (!anyChanged)
            {
                canEvaluateErrored = false;
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
            if (Jobs.Count == 0)
            {
                return;
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

            foreach (var job in Jobs)
            {
                if (job.State != JobState.Idle) continue;

                for (var i = 0; i < entities.Count; i++)
                {
                    var entity = entities[i] as HumanEntity;
                    if (entity == null) continue;
                    if (entity.HasJob) continue;

                    if (jobNoAccessMap[job].Contains(entity))
                    {
                        if (CanEntityReachJob(entity, job))
                        {
                            jobNoAccessMap[job].Remove(entity);
                            AssignEntityJob(entity, job);
                            break;
                        }

                        continue;
                    }

                    if (CanEntityReachJob(entity, job))
                    {
                        AssignEntityJob(entity, job);
                        break;
                    }

                    NotifyWorkerCantReachJob(entity, job);
                }
            }
        }

        /// <summary>
        ///     Notifies the job manager that an active job can no longer be accessed by the
        ///     assigned entity.
        /// </summary>
        /// <param name="_job"></param>
        public void NotifyWorkerCantAccessJob(Job _job)
        {
            NotifyWorkerCantReachJob(_job.AssignedEntity, _job);
            _job.AssignedEntity.SetJob(null, true);
        }

        private void SetJobState(Job _job, JobState _state)
        {
            _job.State = _state;
            jobStateChangedEvent?.Invoke(_job);
        }

        public Tile GetClosestEnterableNeighbour(Entity _entity, IReadOnlyCollection<Tile> _tiles)
        {
            if (_entity == null || _tiles == null || _tiles.Count <= 0) return null;

            Tile closest = null;
            var closestDist = float.MaxValue;

            var entityTile = _entity.CurrentTile;

            foreach (var tile in _tiles)
            {
                if (tile.GetEnterability() == TileEnterability.None) continue;
                if (entityTile.Area == null) continue;
                if (!RegionReachabilityChecker.CanReachRegion(entityTile.Region, tile.Region)) continue;

                var dist = (entityTile.Position - tile.Position).sqrMagnitude;

                // Make tiles that have a job assigned appear more expensive.
                if (tile.CurrentJob != null) dist += 10000.0f;

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = tile;
                }
            }

            return closest;
        }

        private void NotifyWorkerCantReachJob(HumanEntity _entity, Job _job)
        {
            if (jobNoAccessMap[_job].Contains(_entity)) return;
            jobNoAccessMap[_job].Add(_entity);

            // Experimental
            SetJobState(_job, jobNoAccessMap[_job].Count >= World.Instance.Characters.Count
                ? JobState.Error
                : JobState.Idle);
        }

        private bool CanEntityReachJob(Entity _entity, Job _job)
        {
            if (_entity == null || _job == null) return false;

            if (_job.WorkingTile == null || _job.WorkingTile.GetEnterability() == TileEnterability.None)
            {
                _job.WorkingTile = GetClosestEnterableNeighbour(_entity,
                    _job.TargetTile.DirectNeighbours);
                return _job.WorkingTile != null;
            }

            return RegionReachabilityChecker.CanReachRegion(_entity.CurrentTile.Region, _job.WorkingTile.Region);
        }

        public void AddJob(Job _job)
        {
            if (_job == null) return;

            // TODO: Move to mouse controller to visualise duplication for demolish jobs etc.
            if (_job.TargetTile.CurrentJob != null) return;

            jobNoAccessMap.Add(_job, new List<HumanEntity>());

            Jobs.Add(_job);
            _job.TargetTile.CurrentJob = _job;
            _job.State = JobState.Idle;
            jobCreatedEvent?.Invoke(_job);
        }

        public void AddJobs(IEnumerable<Job> _jobs)
        {
            foreach (var job in _jobs)
            {
                AddJob(job);
            }
        }

        public void CancelJob(Job _job)
        {
            Jobs.Remove(_job);
            jobNoAccessMap.Remove(_job);
            // Don't use job.OnComplete here as it actually performs the jobs action.
            _job.AssignedEntity?.SetJob(null, true);
            _job.TargetTile.CurrentJob = null;
            _job.OnCancelled();
            jobCompletedEvent?.Invoke(_job);
        }
    }
}
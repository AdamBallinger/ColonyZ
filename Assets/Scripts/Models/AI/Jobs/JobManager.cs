using System;
using System.Collections.Generic;
using Models.Entities;
using Models.Entities.Living;
using Models.Map;
using Models.Map.Pathing;
using Models.Map.Tiles;

namespace Models.AI.Jobs
{
    public class JobManager
    {
        public static JobManager Instance { get; private set; }
        
        /// <summary>
        /// List of all the current jobs needing to be carried out and have not been assigned to an entity.
        /// This list doesn't know if a job is completable or not.
        /// </summary>
        public List<Job> InactiveJobs { get; private set; }
        
        /// <summary>
        /// List of all jobs currently being worked on by an entity.
        /// </summary>
        public List<Job> ActiveJobs { get; private set; }
        
        /// <summary>
        /// List of all the jobs that are not able to be completed due to either resources missing or
        /// the jobs working tile is not reachable by any available entity.
        /// </summary>
        public List<Job> InvalidJobs { get; private set; }
        
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

        private JobManager() {}

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
            InactiveJobs = new List<Job>();
            ActiveJobs = new List<Job>();  
            InvalidJobs = new List<Job>();
            jobNoAccessMap = new Dictionary<Job, List<HumanEntity>>();
        }
        
        /// <summary>
        /// Event called when an entity has finished a job.
        /// </summary>
        /// <param name="_completedJob"></param>
        private void OnJobFinished(Job _completedJob)
        {
            ActiveJobs.Remove(_completedJob);
            _completedJob.TargetTile.CurrentJob = null;
            _completedJob.OnComplete();
            jobCompletedEvent?.Invoke(_completedJob);

            EvaluateInvalidJobs();
        }
        
        /// <summary>
        /// Checks if any job in the invalid jobs list is now completable.
        /// </summary>
        private void EvaluateInvalidJobs()
        {
            var entities = World.Instance.Characters;
            
            for (var i = InvalidJobs.Count - 1; i >= 0; i--)
            {
                var isJobReachable = false;
                var job = InvalidJobs[i];

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
                    RemoveInvalidJob(job);
                }
            }
        }
        
        private void AssignEntityJob(HumanEntity _entity, Job _job)
        {
            // TODO: Check if job is completable before setting a job (resources etc.).
            if(_entity.SetJob(_job))
            { 
                _job.AssignedEntity = _entity;
                _entity.Motor.SetTargetTile(_job.WorkingTile);
                InactiveJobs.Remove(_job);
                ActiveJobs.Add(_job);
            }
        }
        
        public void Update()
        {
            for(var i = ActiveJobs.Count - 1; i >= 0; i--)
            {
                var job = ActiveJobs[i];
                if (job.Complete)
                {
                    jobNoAccessMap.Remove(job);
                    OnJobFinished(job);
                }
            }
            
            if (InactiveJobs.Count == 0)
            {
                return;
            }

            // TODO: Only get players entities in future.
            var entities = World.Instance.Characters;

            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i] as HumanEntity;
                var job = InactiveJobs[0];
                
                if (entity?.CurrentJob != null || jobNoAccessMap[job].Contains(entity))
                {
                    continue;
                }
                
                var closestTile = GetClosestPathableNeighbour(entity, job.TargetTile.DirectNeighbours);
                
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
                    AddInvalidJob(job);
                }
            }
        }
        
        public bool CanEntityReachJob(Entity _entity, Job _job)
        {
            if (_entity == null || _job == null) return false;

            var closestTile = GetClosestPathableNeighbour(_entity, _job.TargetTile.DirectNeighbours);

            return closestTile != null;
        }

        private bool AddJob(Job _job)
        {
            if (_job == null) return false;

            // TODO: Move to mouse controller to visualise duplication for demolish jobs etc.
            if (_job.TargetTile.CurrentJob != null) return false;
            
            jobNoAccessMap.Add(_job, new List<HumanEntity>());

            _job.TargetTile.CurrentJob = _job;
            InactiveJobs.Add(_job);
            jobCreatedEvent?.Invoke(_job);

            return true;
        }
        
        public void AddJobs(IEnumerable<Job> _jobs)
        {
            var forceEvaluationOfInvalid = false;
            foreach (var job in _jobs)
            {
                if (AddJob(job) && job is DemolishJob) forceEvaluationOfInvalid = true;
            }
            
            if (forceEvaluationOfInvalid) EvaluateInvalidJobs();
        }
        
        /// <summary>
        /// Notifies the job manager that an active job has become invalid.
        /// </summary>
        /// <param name="_job"></param>
        public void NotifyActiveJobInvalid(Job _job)
        {
            if (!ActiveJobs.Contains(_job)) return;

            _job.AssignedEntity.SetJob(null, true);
            
            ActiveJobs.Remove(_job);
            InvalidJobs.Add(_job);
            jobStateChangedEvent?.Invoke(_job);
            EvaluateInvalidJobs();
        }
        
        /// <summary>
        /// Adds a job to the invalid job list, and removes it from the inactive list.
        /// </summary>
        /// <param name="_job"></param>
        private void AddInvalidJob(Job _job)
        {
            InvalidJobs.Add(_job);
            InactiveJobs.Remove(_job);
            jobStateChangedEvent?.Invoke(_job);
        }
        
        /// <summary>
        /// Removes a job from the invalid job list and adds it back into the inactive list.
        /// </summary>
        /// <param name="_job"></param>
        private void RemoveInvalidJob(Job _job)
        {
            InvalidJobs.Remove(_job);
            InactiveJobs.Add(_job);
            jobStateChangedEvent?.Invoke(_job);
        }
        
        public Tile GetClosestEnterableNeighbour(Entity _entity, IReadOnlyCollection<Tile> _tiles)
        {
            if (_entity == null || _tiles == null || _tiles.Count <= 0) return null;
            
            Tile closest = null;
            var closestDist = float.MaxValue;

            var entityTile = _entity.CurrentTile;
                
            foreach(var tile in _tiles)
            {
                if (tile.GetEnterability() != TileEnterability.Immediate) continue;
                if (entityTile.Room == null) continue;
                // Skip the tile if entities current room has no connection to the tiles room.
                if (!entityTile.Room.HasConnection(tile.Room)) continue;
                    
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

        public Tile GetClosestPathableNeighbour(Entity _entity, IReadOnlyCollection<Tile> _tiles)
        {
            if (_entity == null || _tiles == null || _tiles.Count <= 0) return null;
            
            Tile closest = null;
            var closestDist = float.MaxValue;

            var entityTile = _entity.CurrentTile;
                
            foreach(var tile in _tiles)
            {
                if (tile.GetEnterability() != TileEnterability.Immediate) continue;
                if (entityTile.Room == null) continue;
                // Skip the tile if entities current room has no connection to the tiles room.
                if (!entityTile.Room.HasConnection(tile.Room)) continue;
                    
                var dist = (entityTile.Position - tile.Position).sqrMagnitude;

                // Make tiles that have a job assigned appear more expensive.
                if (tile.CurrentJob != null) dist += 1000.0f;

                if (dist < closestDist && PathFinder.TestPath(_entity.CurrentTile, tile))
                {
                    closestDist = dist;
                    closest = tile;
                }
            }

            return closest;
        }
    }
}
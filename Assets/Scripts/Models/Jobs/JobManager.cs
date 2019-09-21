using System;
using System.Collections.Generic;
using Models.Entities.Living;
using Models.Map;
using Models.Map.Pathing;

namespace Models.Jobs
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
                //Debug.Log($"Current: {i} of {InvalidJobs.Count} invalid jobs.");
                var job = InvalidJobs[i];
                var jobNowValid = false;

                foreach (var livingEntity in entities)
                {
                    var humanEntity = livingEntity as HumanEntity;
                    
                    foreach (var tile in job.TargetTile.DirectNeighbours)
                    {
                        if (PathFinder.TestPath(humanEntity?.CurrentTile, tile))
                        {
                            jobNowValid = true;
                            RemoveInvalidJob(job);
                            break;
                        }
                    }

                    if (jobNowValid) break;
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
                if (job.Progress >= 1.0f)
                {
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
                if (entity?.CurrentJob != null)
                {
                    continue;
                }

                var job = InactiveJobs[0];
                var jobAssigned = false;
                
                foreach (var tile in job.TargetTile.DirectNeighbours)
                {
                    if (PathFinder.TestPath(entity?.CurrentTile, tile))
                    {
                        job.WorkingTile = tile;
                        AssignEntityJob(entity, job);
                        jobAssigned = true;
                        break;
                    }
                }

                if (jobAssigned) break;

                // If the last entity can't reach the job, then the job must be unreachable.
                if (i == entities.Count - 1)
                {
                    AddInvalidJob(job);
                }
            }
        }

        public void AddJob(Job _job)
        {
            if (_job == null) return;

            _job.TargetTile.CurrentJob = _job;
            InactiveJobs.Add(_job);
            jobCreatedEvent?.Invoke(_job);
        }
        
        /// <summary>
        /// Notifies the job manager that an active job has become invalid.
        /// </summary>
        /// <param name="_job"></param>
        public void NotifyActiveJobInvalid(Job _job)
        {
            if (!ActiveJobs.Contains(_job)) return;
            
            ActiveJobs.Remove(_job);
            InvalidJobs.Add(_job);
            jobStateChangedEvent?.Invoke(_job);
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
    }
}
using System.Collections.Generic;
using Models.Entities.Living;
using Models.Map;

namespace Models.Jobs
{
    public class JobManager
    {
        public static JobManager Instance { get; private set; }
        
        /// <summary>
        /// List of all the current jobs needing to be carried out and have not been assigned to an entity.
        /// This list doesn't know if a job is completable or not.
        /// </summary>
        private List<Job> InactiveJobs { get; set; }
        
        /// <summary>
        /// List of all jobs currently being worked on by an entity.
        /// </summary>
        private List<Job> ActiveJobs { get; set; }
        
        /// <summary>
        /// List of all the jobs that are not able to be completed due to either resources missing or
        /// the jobs working tile is not reachable by any available entity.
        /// </summary>
        private List<Job> InvalidJobs { get; set; }

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
            _completedJob.OnComplete();
            ActiveJobs.Remove(_completedJob);

            EvaluateInvalidJobs();
        }
        
        /// <summary>
        /// Checks if any job in the invalid jobs list is now completable.
        /// </summary>
        private void EvaluateInvalidJobs()
        {
            
        }
        
        private void AssignEntityJob(HumanEntity _entity, Job _job)
        {
            // TODO: Check if job is completable before setting a job.
            if(_entity.SetJob(_job))
            {
                _job.AssignedEntity = _entity;
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
            
            // TODO: Distribute jobs to available entities.
            var entities = World.Instance.Characters; // TODO: For now this will work, but in future need to get only players human entities.

            foreach (var livingEntity in entities)
            {
                var entity = (HumanEntity) livingEntity;
                if (entity.CurrentJob != null)
                {
                    continue;
                }
                
                AssignEntityJob(entity, InactiveJobs[0]);
            }
        }
        
        public void AddJob(Job _job)
        {
            if (_job == null) return;
            
            InactiveJobs.Add(_job);
        }
    }
}
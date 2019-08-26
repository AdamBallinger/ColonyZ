using System.Collections.Generic;
using Models.Entities.Living;

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
                InactiveJobs.Remove(_job);
                ActiveJobs.Add(_job);
            }
        }
        
        public void Update()
        {
            foreach(var job in ActiveJobs)
            {
                if (job.Progress >= 1.0f)
                {
                    OnJobFinished(job);
                }
            }
            
            // TODO: Distribute jobs to available entities.
        }
        
        public void AddJob(Job _job)
        {
            if (_job == null) return;
            
            InactiveJobs.Add(_job);
        }
    }
}
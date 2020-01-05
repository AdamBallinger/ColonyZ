using System.Collections.Generic;
using Models.AI.Jobs;
using UnityEngine;

namespace Controllers.UI.Jobs
{
    public class JobListController : MonoBehaviour
    {
        [SerializeField]
        private GameObject jobEntryPrefab;

        [SerializeField]
        private GameObject entryContainer;

        private Dictionary<Job, GameObject> jobEntryMap;
        
        private void Start()
        {
            jobEntryMap = new Dictionary<Job, GameObject>();
            
            JobManager.Instance.jobCreatedEvent += CreateEntry;
            JobManager.Instance.jobStateChangedEvent += UpdateEntry;
            JobManager.Instance.jobCompletedEvent += DeleteEntry;
        }

        private void CreateEntry(Job _job)
        {
            if (jobEntryMap.ContainsKey(_job)) return;

            var entry = Instantiate(jobEntryPrefab, entryContainer.transform);
            entry.GetComponent<JobEntryController>().Set(_job);
            
            jobEntryMap.Add(_job, entry);
        }
        
        private void UpdateEntry(Job _job)
        {
            if (!jobEntryMap.ContainsKey(_job)) return;
            
            jobEntryMap[_job].GetComponent<JobEntryController>().Set(_job);
        }
        
        private void DeleteEntry(Job _job)
        {
            if (!jobEntryMap.ContainsKey(_job)) return;
            
            Destroy(jobEntryMap[_job]);
            jobEntryMap.Remove(_job);
        }
    }
}
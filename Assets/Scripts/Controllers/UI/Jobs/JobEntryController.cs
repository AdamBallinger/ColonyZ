using Models.Jobs;
using TMPro;
using UnityEngine;

namespace Controllers.UI.Jobs
{
    public class JobEntryController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text jobNameText;
        
        public void Set(Job _job)
        {
            jobNameText.color = JobManager.Instance.InvalidJobs.Contains(_job) ? Color.red : Color.white;
            jobNameText.text = _job.JobName;
        }
    }
}
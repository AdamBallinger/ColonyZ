using Models.AI.Jobs;
using TMPro;
using UnityEngine;

namespace Controllers.UI.Jobs
{
    public class JobEntryController : MonoBehaviour
    {
        [SerializeField] private TMP_Text jobNameText;

        private Job job;

        public void Set(Job _job)
        {
            job = _job;
            JobManager.Instance.jobStateChangedEvent += StateChanged;
            jobNameText.text = _job.JobName;
        }

        private void StateChanged(Job _job)
        {
            if (_job == job)
            {
                jobNameText.color = job.State == JobState.Active ? Color.green :
                    job.State == JobState.Error ? Color.red : Color.gray;
            }
        }
    }
}
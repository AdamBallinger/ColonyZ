using ColonyZ.Models.AI.Jobs;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.UI.Jobs
{
    public class JobEntryController : MonoBehaviour
    {
        [SerializeField] private TMP_Text jobNameText;

        public void Set(Job _job)
        {
            jobNameText.text = _job.JobName;
            jobNameText.color = _job.State == JobState.Active ? Color.green :
                _job.State == JobState.Error ? Color.red : Color.gray;
        }
    }
}
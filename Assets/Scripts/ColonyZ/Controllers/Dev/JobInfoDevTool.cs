using System.Linq;
using ColonyZ.Models.AI.Jobs;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.Dev
{
    public class JobInfoDevTool : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            JobManager.Instance.jobCreatedEvent += j => UpdateInfo();
            JobManager.Instance.jobStateChangedEvent += j => UpdateInfo();
            JobManager.Instance.jobCompletedEvent += j => UpdateInfo();
        }

        private void UpdateInfo()
        {
            var availabelCharacters = World.Instance.Characters.Cast<HumanEntity>()
                .Count(h => h.CurrentJob == null);
            text.text = $"Job count: {JobManager.Instance.JobCount}\n" +
                        $"Jobs Active: {JobManager.Instance.ActiveCount}\n" +
                        $"Jobs Idle: {JobManager.Instance.IdleCount}\n" +
                        $"Jobs Errored: {JobManager.Instance.ErrorCount}\n" +
                        $"Available characters: {availabelCharacters}";
        }
    }
}
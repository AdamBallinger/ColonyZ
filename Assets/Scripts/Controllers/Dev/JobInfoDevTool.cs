using System.Linq;
using Models.AI.Jobs;
using Models.Entities.Living;
using Models.Map;
using TMPro;
using UnityEngine;

namespace Controllers.Dev
{
    public class JobInfoDevTool : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Update()
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
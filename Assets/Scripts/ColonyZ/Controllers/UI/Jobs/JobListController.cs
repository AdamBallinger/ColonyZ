using System.Collections.Generic;
using ColonyZ.Models.AI.Jobs;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.UI.Jobs
{
    public class JobListController : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonText;

        [SerializeField] private Color closeTextColor;

        [SerializeField] private GameObject entryContainer;

        [SerializeField] private GameObject entryRoot;

        private bool isOpen;

        private Dictionary<Job, JobEntryController> jobEntryMap;
        [SerializeField] private GameObject jobEntryPrefab;

        [SerializeField] private Color openTextColor;

        private RectTransform rTransform;

        public void Init()
        {
            jobEntryMap = new Dictionary<Job, JobEntryController>();
            rTransform = GetComponent<RectTransform>();
            Close();

            JobManager.Instance.jobCreatedEvent += CreateEntry;
            JobManager.Instance.jobStateChangedEvent += UpdateEntry;
            JobManager.Instance.jobCompletedEvent += DeleteEntry;
        }

        private void CreateEntry(Job _job)
        {
            if (jobEntryMap.ContainsKey(_job)) return;

            var entry = Instantiate(jobEntryPrefab, entryContainer.transform);
            var ec = entry.GetComponent<JobEntryController>();
            ec.Set(_job);

            jobEntryMap.Add(_job, ec);
        }

        private void UpdateEntry(Job _job)
        {
            if (!jobEntryMap.ContainsKey(_job)) return;

            jobEntryMap[_job].Set(_job);
        }

        private void DeleteEntry(Job _job)
        {
            if (!jobEntryMap.ContainsKey(_job)) return;

            Destroy(jobEntryMap[_job].gameObject);
            jobEntryMap.Remove(_job);
        }

        public void OnWindowToggleClick()
        {
            if (isOpen) Close();
            else Open();
        }

        private void Open()
        {
            isOpen = true;
            buttonText.text = "Close";
            buttonText.color = closeTextColor;
            entryRoot.SetActive(true);
            rTransform.sizeDelta = new Vector2(311, 490);
        }

        private void Close()
        {
            isOpen = false;
            buttonText.text = "Open";
            buttonText.color = openTextColor;
            entryRoot.SetActive(false);
            rTransform.sizeDelta = new Vector2(240, 21);
        }
    }
}
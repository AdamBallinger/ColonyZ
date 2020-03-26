using System.Collections.Generic;
using Models.AI.Jobs;
using TMPro;
using UnityEngine;

namespace Controllers.UI.Jobs
{
    public class JobListController : MonoBehaviour
    {
        [SerializeField] private GameObject jobEntryPrefab;

        [SerializeField] private GameObject entryContainer;

        [SerializeField] private TMP_Text buttonText;

        [SerializeField] private Color closeTextColor;

        [SerializeField] private Color openTextColor;

        [SerializeField] private GameObject entryRoot;

        private Dictionary<Job, GameObject> jobEntryMap;

        private RectTransform rTransform;

        private bool isOpen;

        private void Start()
        {
            jobEntryMap = new Dictionary<Job, GameObject>();
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
            rTransform.sizeDelta = new Vector2(254, 407);
        }

        private void Close()
        {
            isOpen = false;
            buttonText.text = "Open";
            buttonText.color = openTextColor;
            entryRoot.SetActive(false);
            rTransform.sizeDelta = new Vector2(172, 27);
        }
    }
}
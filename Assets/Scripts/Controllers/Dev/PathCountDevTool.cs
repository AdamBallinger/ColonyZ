using Models.Map.Pathing;
using TMPro;
using UnityEngine;

namespace Controllers.Dev
{
    public class PathCountDevTool : MonoBehaviour
    {
        public TMP_Text text;

        private void Update()
        {
            text.text = $"Queued Paths: {PathFinder.Instance.TaskCount.ToString()}";
        }
    }
}
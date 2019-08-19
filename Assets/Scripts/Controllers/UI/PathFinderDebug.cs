using Models.Map.Pathing;
using TMPro;
using UnityEngine;

namespace Controllers.UI
{
    public class PathFinderDebug : MonoBehaviour
    {
        public TMP_Text text;
        
        private void Update()
        {
            text.text = PathFinder.Instance.TaskCount.ToString();
        }
    }
}

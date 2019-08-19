using Models.TimeSystem;
using TMPro;
using UnityEngine;

namespace Controllers.UI.Time
{
    public class ClockController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text clockText;

        [SerializeField]
        private TMP_Text modeText;

        // TODO: Change to an event for when the minute/hour changes instead of each frame. Lots of garbage allocated here.
        private void Update()
        {
            clockText.text = $"{TimeManager.Instance.Hour:00}:{TimeManager.Instance.Minute:00}";
            modeText.text = $"{(int)TimeManager.Instance.TimeMode}x";
        }
    }
}

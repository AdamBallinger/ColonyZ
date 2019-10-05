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
        
        private void Start()
        {
            TimeManager.Instance.timeModeChangedEvent += OnTimeModeChanged;
            TimeManager.Instance.timeChangedEvent += OnTimeChanged;

            modeText.text = $"{(int) TimeManager.Instance.TimeMode}x";
        }
        
        private void OnTimeModeChanged(TimeMode _newMode)
        {
            modeText.text = $"{(int)_newMode}x";
        }
        
        private void OnTimeChanged(int _hour, int _minute)
        {
            clockText.text = $"{_hour:00}:{_minute:00}";
        }
    }
}

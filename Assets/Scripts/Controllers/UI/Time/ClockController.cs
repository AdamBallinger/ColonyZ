using Models.TimeSystem;
using TMPro;
using UnityEngine;

namespace Controllers.UI.Time
{
    public class ClockController : MonoBehaviour
    {
        [SerializeField] private TMP_Text clockText;

        [SerializeField] private TMP_Text modeText;

        [SerializeField] private TMP_Text dayText;

        [SerializeField] private TMP_Text temperatureText;

        [SerializeField] private TMP_Text godModeText;

        private void Start()
        {
            TimeManager.Instance.timeModeChangedEvent += OnTimeModeChanged;
            TimeManager.Instance.timeChangedEvent += OnTimeChanged;
            TimeManager.Instance.newDayEvent += day => dayText.text = "Day: " + day;
            MouseController.Instance.BuildModeController.godModeChangeEvent += OnGodModeChange;

            modeText.text = $"{(int) TimeManager.Instance.TimeMode}x";
            dayText.text = "Day: " + TimeManager.Instance.Day;
            OnGodModeChange(MouseController.Instance.BuildModeController.GodMode);
        }

        private void OnGodModeChange(bool _state)
        {
            godModeText.text = _state ? "GOD MODE" : string.Empty;
        }

        private void OnTimeModeChanged(TimeMode _newMode)
        {
            modeText.text = $"{(int) _newMode}x";
        }

        private void OnTimeChanged(int _hour, int _minute)
        {
            clockText.text = $"{_hour:00}:{_minute:00}";
        }
    }
}
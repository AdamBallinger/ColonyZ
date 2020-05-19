using ColonyZ.Models.TimeSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI.Time
{
    public class TimeControlsController : MonoBehaviour
    {
        [SerializeField] private Button decreaseButton;

        [SerializeField] private Button increaseButton;

        [SerializeField] private Button playPauseButton;

        [SerializeField] private Image playPauseImage;

        [SerializeField] private Sprite[] playPauseSprites;

        private TimeManager timeManager;

        private void Start()
        {
            timeManager = TimeManager.Instance;
            playPauseImage.sprite = timeManager.TimeMode == TimeMode.x0 ? playPauseSprites[0] : playPauseSprites[1];

            timeManager.timeModeChangedEvent += OnTimeModeChanged;

            increaseButton.onClick.AddListener(OnTimeIncrease);
            decreaseButton.onClick.AddListener(OnTimeDecrease);
            playPauseButton.onClick.AddListener(OnPlayResume);
        }

        /// <summary>
        ///     Event called when the increase time button is pressed.
        /// </summary>
        private void OnTimeIncrease()
        {
            switch (timeManager.TimeMode)
            {
                case TimeMode.x0:
                    timeManager.TimeMode = TimeMode.x1;
                    break;
                case TimeMode.x1:
                    timeManager.TimeMode = TimeMode.x2;
                    break;
                case TimeMode.x2:
                    timeManager.TimeMode = TimeMode.x4;
                    break;
                case TimeMode.x4:
                    timeManager.TimeMode = TimeMode.x10;
                    break;
            }
        }

        /// <summary>
        ///     Event called when the decrease time button is pressed.
        /// </summary>
        private void OnTimeDecrease()
        {
            switch (timeManager.TimeMode)
            {
                case TimeMode.x10:
                    timeManager.TimeMode = TimeMode.x4;
                    break;
                case TimeMode.x4:
                    timeManager.TimeMode = TimeMode.x2;
                    break;
                case TimeMode.x2:
                    timeManager.TimeMode = TimeMode.x1;
                    break;
            }
        }

        /// <summary>
        ///     Event called when the play/pause time button is pressed.
        /// </summary>
        private void OnPlayResume()
        {
            timeManager.Toggle();
        }

        private void OnTimeModeChanged(TimeMode _newMode)
        {
            increaseButton.interactable = _newMode != TimeMode.x10 || _newMode != TimeMode.x0;
            decreaseButton.interactable = _newMode != TimeMode.x1 || _newMode != TimeMode.x0;
            playPauseImage.sprite = _newMode == TimeMode.x0 ? playPauseSprites[0] : playPauseSprites[1];
        }
    }
}
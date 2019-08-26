using Models.TimeSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI.Time
{
    public class TimeControlsController : MonoBehaviour
    {
        [SerializeField]
        private Button increaseButton;

        [SerializeField]
        private Button decreaseButton;

        [SerializeField]
        private Image playPauseImage;

        [SerializeField]
        private Sprite[] playPauseSprites;
        
        private void Start()
        {
            var tm = TimeManager.Instance;
            playPauseImage.sprite = tm.TimeMode == TimeMode.x0 ? playPauseSprites[0] : playPauseSprites[1];
        }
        
        /// <summary>
        /// Event called when the increase time button is pressed.
        /// </summary>
        public void OnTimeIncrease()
        {
            var tm = TimeManager.Instance;
            
            switch (tm.TimeMode)
            {
                case TimeMode.x0:
                    tm.TimeMode = TimeMode.x1;
                    decreaseButton.interactable = true;
                    break;
                case TimeMode.x1:
                    tm.TimeMode = TimeMode.x2;
                    decreaseButton.interactable = true;
                    break;
                case TimeMode.x2:
                    tm.TimeMode = TimeMode.x4;
                    increaseButton.interactable = false;
                    break;
            }
        }
        
        /// <summary>
        /// Event called when the decrease time button is pressed.
        /// </summary>
        public void OnTimeDecrease()
        {
            var tm = TimeManager.Instance;
            
            switch (tm.TimeMode)
            {
                case TimeMode.x4:
                    tm.TimeMode = TimeMode.x2;
                    increaseButton.interactable = true;
                    break;
                case TimeMode.x2:
                    tm.TimeMode = TimeMode.x1;
                    decreaseButton.interactable = false;
                    break;
            }
        }
        
        /// <summary>
        /// Event called when the pause/resume time button is pressed.
        /// </summary>
        public void OnPauseResume()
        {
            var tm = TimeManager.Instance;
            tm.TimeMode = tm.TimeMode == TimeMode.x0 ? TimeMode.x1 : TimeMode.x0;
            increaseButton.interactable = tm.TimeMode != TimeMode.x4;
            decreaseButton.interactable = tm.TimeMode != TimeMode.x0;
            playPauseImage.sprite = tm.TimeMode == TimeMode.x0 ? playPauseSprites[0] : playPauseSprites[1];
        }
    }
}

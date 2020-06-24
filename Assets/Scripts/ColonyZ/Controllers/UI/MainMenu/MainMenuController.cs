using ColonyZ.Models.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button loadGameButton;

        [SerializeField] private GameObject mainButtonsParent;
        [SerializeField] private GameObject sizeSelectionParent;

        private void Start()
        {
            loadGameButton.interactable = SaveGameHandler.SaveGamePresent();
        }

        public void OnNewGamePressed()
        {
            WorldLoadSettings.LOAD_TYPE = WorldLoadType.New;
            sizeSelectionParent.SetActive(true);
            mainButtonsParent.SetActive(false);
        }

        public void OnLoadGamePressed()
        {
            WorldLoadSettings.LOAD_TYPE = WorldLoadType.Load;
            SceneManager.LoadScene("world");
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }

        public void BackToMainMenu()
        {
            sizeSelectionParent.SetActive(false);
            mainButtonsParent.SetActive(true);
        }
    }
}
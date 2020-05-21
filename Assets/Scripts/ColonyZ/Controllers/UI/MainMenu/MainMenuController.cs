using ColonyZ.Models.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button loadGameButton;

        private void Start()
        {
            loadGameButton.interactable = SaveGameHandler.SaveGamePresent();
        }

        public void OnNewGamePressed()
        {
            WorldController.LOADING_TYPE = WorldLoadType.New;
            SceneManager.LoadScene("world");
        }

        public void OnLoadGamePressed()
        {
            WorldController.LOADING_TYPE = WorldLoadType.Load;
            SceneManager.LoadScene("world");
        }

        public void OnQuitPressed()
        {
            Application.Quit();
        }
    }
}
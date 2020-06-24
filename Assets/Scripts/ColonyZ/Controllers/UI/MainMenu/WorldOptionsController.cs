using ColonyZ.Models.Map;
using ColonyZ.Models.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColonyZ.Controllers.UI.MainMenu
{
    public class WorldOptionsController : MonoBehaviour
    {
        [SerializeField] private GameObject worldSizeButtonPrefab;
        [SerializeField] private RectTransform sizeButtonParent;

        private void Start()
        {
            foreach (var size in WorldSizeTypes.SIZES)
            {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
                if (!size.Available) continue;
#endif
                var button = Instantiate(worldSizeButtonPrefab, sizeButtonParent);
                button.GetComponent<WorldSizeButtonController>().SetSize(size, OnSizeSelected);
            }
        }

        private void OnSizeSelected(WorldSizeTypes.WorldSize _size)
        {
            WorldLoadSettings.WORLD_SIZE = _size;
            SceneManager.LoadScene("world");
        }
    }
}
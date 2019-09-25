using Models.Map.Tiles;
using Models.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI
{
    public class SelectionController : MonoBehaviour
    {
        /// <summary>
        /// Determines if the selection container is visible.
        /// </summary>
        public bool IsVisible { get; private set; }
        
        [SerializeField]
        private GameObject selectionContainer;
        
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TMP_Text title;
        
        [SerializeField]
        private TMP_Text description;
        
        /// <summary>
        /// Called when the mouse clicks on a tile.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileSelect(Tile _tile)
        {
            Set(_tile);
        }
        
        private void Set(ISelectable _selectable)
        {
            icon.sprite = _selectable.GetSelectionIcon();
            title.text = _selectable.GetSelectionName();
            description.text = _selectable.GetSelectionDescription();
            IsVisible = true;
            selectionContainer.SetActive(true);
        }
        
        private void Update()
        {
            if (IsVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                selectionContainer.SetActive(false);
                IsVisible = false;
            }
        }
    }
}
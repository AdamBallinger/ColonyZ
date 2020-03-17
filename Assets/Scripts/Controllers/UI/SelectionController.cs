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
        private GameObject selectionObject;
        
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TMP_Text title;
        
        [SerializeField]
        private TMP_Text description;

        private ISelectable currentSelection;
        
        /// <summary>
        /// Called when the mouse clicks on a tile.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileSelect(Tile _tile)
        {
            if (_tile == null)
            {
                HideSelectable();
                return;
            }
            
            if (_tile.GetItemStack() != null)
            {
                Set(_tile.Item);
            }
            else if (_tile.LivingEntities.Count > 0)
            {
                Set(_tile.LivingEntities[0]);
            }
            else if (_tile.HasObject)
            {
                Set(_tile.Object);
            }
            else
            {
                Set(_tile);
            }
        }
        
        private void Set(ISelectable _selectable)
        {
            currentSelection = _selectable;
            icon.sprite = _selectable.GetSelectionIcon();
            title.text = _selectable.GetSelectionName();
            description.text = _selectable.GetSelectionDescription();
            IsVisible = true;
            selectionContainer.SetActive(true);
            selectionObject.SetActive(true);
            selectionObject.transform.position = currentSelection.GetPosition();
        }
        
        private void Update()
        {
            if (IsVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                HideSelectable();
            }
            
            if (IsVisible)
            {
                description.text = currentSelection.GetSelectionDescription();
                selectionObject.transform.position = currentSelection.GetPosition();
            }
        }
        
        private void HideSelectable()
        {
            IsVisible = false;
            selectionContainer.SetActive(false);
            selectionObject.SetActive(false);
        }
    }
}
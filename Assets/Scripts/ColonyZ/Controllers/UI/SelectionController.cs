using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI
{
    public class SelectionController : MonoBehaviour
    {
        public static ISelectable currentSelection;

        [SerializeField] private TMP_Text description;

        [SerializeField] private Image icon;

        [SerializeField] private GameObject selectionContainer;

        [SerializeField] private GameObject selectionObject;

        [SerializeField] private TMP_Text title;

        /// <summary>
        ///     Determines if the selection container is visible.
        /// </summary>
        private bool IsVisible { get; set; }

        /// <summary>
        ///     Called when the mouse clicks on a tile.
        /// </summary>
        /// <param name="_tile"></param>
        public void OnTileSelect(Tile _tile)
        {
            if (_tile == null)
            {
                HideSelectionInfo();
                return;
            }

            if (_tile.GetItemStack() != null)
                Set(_tile.Item);
            else if (_tile.LivingEntities.Count > 0)
                Set(_tile.LivingEntities[0]);
            else if (_tile.HasObject)
                Set(_tile.Object);
            else
                Set(_tile);
        }

        public void SetCursor(Vector2 _pos)
        {
            selectionObject.SetActive(true);
            selectionObject.transform.position = _pos;
        }

        public void HideCursor()
        {
            selectionObject.SetActive(false);
        }

        private void HideSelectionInfo()
        {
            IsVisible = false;
            currentSelection = null;
            selectionContainer.SetActive(false);
            HideCursor();
        }

        private void Set(ISelectable _selectable)
        {
            currentSelection = _selectable;
            icon.sprite = _selectable.GetSelectionIcon();
            title.text = _selectable.GetSelectionName();
            description.text = _selectable.GetSelectionDescription();
            IsVisible = true;
            selectionContainer.SetActive(true);
            SetCursor(_selectable.GetPosition());
        }

        private void Update()
        {
            if (IsVisible && Input.GetKeyDown(KeyCode.Escape)) HideSelectionInfo();

            if (IsVisible)
            {
                description.text = currentSelection.GetSelectionDescription();
                selectionObject.transform.position = currentSelection.GetPosition();
            }
        }
    }
}
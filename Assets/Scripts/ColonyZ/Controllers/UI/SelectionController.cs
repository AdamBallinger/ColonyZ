using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Models.UI;
using ColonyZ.Utils;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.UI
{
    public class SelectionController : MonoBehaviour
    {
        public static ISelectable currentSelection;

        [SerializeField] private TMP_Text description;

        [SerializeField] private GameObject selectionContainer;

        [SerializeField] private GameObject selectionObject;

        [SerializeField] private TMP_Text title;

        private SpriteRenderer selectionSpriteRenderer;

        /// <summary>
        ///     Determines if the selection container is visible.
        /// </summary>
        private bool IsVisible { get; set; }

        private void Start()
        {
            selectionSpriteRenderer = selectionObject.GetComponent<SpriteRenderer>();
        }

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

            if (_tile.Zone != null)
            {
                Set(_tile.Zone);
                ZoneManager.Instance.CurrentZoneBeingModified = _tile.Zone;
            }
            else if (_tile.GetItemStack() != null)
                Set(_tile.Item);
            else if (_tile.LivingEntities.Count > 0)
                Set(_tile.LivingEntities[0]);
            else if (_tile.HasObject)
                Set(_tile.Object);
        }

        private void SetCursor(ISelectable _selectable)
        {
            selectionObject.SetActive(!(_selectable is Zone));
            selectionObject.transform.position = _selectable.GetPosition();

            if (_selectable is TileObject obj)
            {
                var w = ObjectRotationUtil.GetRotatedObjectWidth(obj);
                var h = ObjectRotationUtil.GetRotatedObjectHeight(obj);
                var size = selectionSpriteRenderer.size;
                var pos = selectionObject.transform.position;
                size.x = w;
                size.y = h;
                pos.x += 0.5f * (w - 1);
                pos.y -= 0.5f * (h - 1);
                selectionSpriteRenderer.size = size;
                selectionObject.transform.position = pos;
            }
        }

        public void SetCursor(Tile _tile)
        {
            if (_tile == null)
            {
                selectionObject.SetActive(false);
                return;
            }

            selectionObject.SetActive(true);
            selectionObject.transform.position = _tile.Position;
            selectionSpriteRenderer.size = Vector2.one;
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
            title.text = _selectable.GetSelectionName();
            description.text = _selectable.GetSelectionDescription();
            selectionSpriteRenderer.size = Vector2.one;
            IsVisible = true;
            selectionContainer.SetActive(true);
            SetCursor(_selectable);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) HideSelectionInfo();

            if (currentSelection == null)
            {
                HideSelectionInfo();
                return;
            }

            if (IsVisible)
            {
                description.text = currentSelection.GetSelectionDescription();
                if (!(currentSelection is TileObject))
                    selectionObject.transform.position = currentSelection.GetPosition();
            }
        }
    }
}
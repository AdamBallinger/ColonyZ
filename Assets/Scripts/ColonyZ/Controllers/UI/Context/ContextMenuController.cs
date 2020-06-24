using System.Collections.Generic;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.UI.Context;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace ColonyZ.Controllers.UI.Context
{
    public class ContextMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject contextMenuButtonPrefab;
        [SerializeField] private GameObject contextWindowRoot;
        [SerializeField] private GameObject buttonContainer;

        [SerializeField] private TMP_Text menuTitleText;

        private List<GameObject> menuButtons = new List<GameObject>();

        private RectTransform contextWindowTransform;

        private Camera cam;

        private SelectionController selectionController;

        /// <summary>
        /// Reference to the object the context menu is currently open for.
        /// </summary>
        private IContextProvider currentProvider;

        /// <summary>
        /// The world position the context menu should be anchored to.
        /// </summary>
        private Vector2 anchorPos;

        private void Start()
        {
            MouseController.Instance.mouseClickEvent += OnMouseClick;

            contextWindowTransform = contextWindowRoot.GetComponent<RectTransform>();
            cam = Camera.main;
            selectionController = FindObjectOfType<SelectionController>();
        }

        private void OpenContextMenu(IContextProvider _contextProvider)
        {
            if (_contextProvider == null)
            {
                CloseContextMenu();
                return;
            }

            if (currentProvider == _contextProvider) return;
            if (currentProvider != _contextProvider) CloseContextMenu();

            contextWindowRoot.SetActive(true);

            currentProvider = _contextProvider;
            menuTitleText.text = _contextProvider.GetContextMenuName();

            foreach (var action in _contextProvider.GetContextActions())
            {
                var btn = Instantiate(contextMenuButtonPrefab, buttonContainer.transform);
                var btnC = btn.GetComponent<ContextMenuButton>();
                action.Action += CloseContextMenu;
                btnC.Set(action);
                menuButtons.Add(btn);
            }
        }

        private void CloseContextMenu()
        {
            if (currentProvider == null) return;

            currentProvider = null;

            for (var i = menuButtons.Count - 1; i >= 0; i--)
            {
                Destroy(menuButtons[i]);
            }

            contextWindowRoot.SetActive(false);
            selectionController.HideCursor();
        }

        private void OnMouseClick(MouseButton _btn, Tile _tile, bool _onUI)
        {
            if (_btn != MouseButton.RightMouse) return;

            if (_tile == null || _tile.IsMapEdge)
            {
                CloseContextMenu();
                return;
            }

            anchorPos = _tile.Position + new Vector2(0.5f, 0.0f);
            RepositionMenu();

            if (_tile.HasObject) OpenContextMenu(_tile.Object);
            else if (_tile.LivingEntities.Count > 0) OpenContextMenu(_tile.LivingEntities[0]);
            else CloseContextMenu();

            if (currentProvider != null && !(currentProvider is LivingEntity))
                selectionController.SetCursor(_tile.Position);
            else selectionController.HideCursor();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseContextMenu();
            }

            if (contextWindowRoot.activeSelf)
            {
                RepositionMenu();
            }
        }

        private void RepositionMenu()
        {
            Vector2 tileScreenPos = cam.WorldToScreenPoint(anchorPos);
            contextWindowTransform.position = tileScreenPos;
        }
    }
}
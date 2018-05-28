using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class ToolbarController : MonoBehaviour
    {
        public static ToolbarController Instance { get; private set; }

        [SerializeField]
        private GameObject toolbarButtonPrefab = null;

        [SerializeField]
        private GameObject toolbarTabButtonPrefab = null;

        [SerializeField]
        private GameObject toolbarSubMenuItemPrefab = null;

        /// <summary>
        /// Reference to the gameobject that parents all of the toolbar menu buttons. E.g. Construction, Commands, etc.
        /// </summary>
        [SerializeField]
        private GameObject toolBarButtonsParent = null;

        /// <summary>
        /// Reference to the gameobject that parents all the sub menu buttons. E.g For construction: Structures, Areas, etc.
        /// </summary>
        [SerializeField]
        private GameObject toolbarSubMenuParent = null;

        /// <summary>
        /// Reference to the gameobject that parents all the sub menu item buttons. E.g. construction -> building -> Wood Wall, Steel Wall etc.
        /// </summary>
        [SerializeField]
        private GameObject toolbarSubMenuItemsParent = null;

        /// <summary>
        /// Maps each root menu to its sub menu container.
        /// </summary>
        private Dictionary<string, ToolbarSubMenuContainer> subMenuMap = new Dictionary<string, ToolbarSubMenuContainer>();

        private string currentSubMenu = string.Empty;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            AddRootMenu("Construction");
            AddRootMenu("Commands", false);
            AddRootMenu("Menu");

            AddSubMenu("Construction", "Building");
            AddSubMenu("Construction", "Area");

            AddSubMenu("Menu", "Exit", Application.Quit);
        }

        /// <summary>
        /// Add a new button to the root of the UI toolbar.
        /// </summary>
        /// <param name="_menuName">Button text.</param>
        /// <param name="_enabledByDefault">Whether this button is enabled by default.</param>
        public void AddRootMenu(string _menuName, bool _enabledByDefault = true)
        {
            var button = Instantiate(toolbarButtonPrefab, toolBarButtonsParent.transform);
            var buttonController = button.GetComponent<ToolbarButtonConroller>();
            buttonController.SetButtonText(_menuName);
            buttonController.SetButtonState(_enabledByDefault);
            buttonController.AddButtonClickAction(() =>
            {
                if (currentSubMenu == _menuName)
                {
                    currentSubMenu = string.Empty;
                    subMenuMap[_menuName].SetMenuButtonsState(false);
                }
                else
                {
                    if (subMenuMap.ContainsKey(currentSubMenu))
                    {
                        subMenuMap[currentSubMenu].SetMenuButtonsState(false);
                    }

                    currentSubMenu = _menuName;
                    subMenuMap[_menuName].SetMenuButtonsState(true);
                }
            });
        }

        /// <summary>
        /// Adds a sub menu for a root menu button.
        /// </summary>
        /// <param name="_rootMenu"></param>
        /// <param name="_subMenuName"></param>
        /// <param name="_clickAction"></param>
        public void AddSubMenu(string _rootMenu, string _subMenuName, Action _clickAction = null)
        {
            if (!subMenuMap.ContainsKey(_rootMenu))
            {
                subMenuMap.Add(_rootMenu, new ToolbarSubMenuContainer());
            }

            var button = Instantiate(toolbarTabButtonPrefab, toolbarSubMenuParent.transform);
            button.SetActive(false);
            var buttonController = button.GetComponent<ToolbarButtonConroller>();
            buttonController.SetButtonText(_subMenuName);
            buttonController.AddButtonClickAction(_clickAction);

            // TODO: Add button action (lambda) to display sub menu items to sub menu items panel when clicked.

            subMenuMap[_rootMenu].AddButton(_subMenuName, buttonController);
        }

        /// <summary>
        /// Adds an item button for a specified sub menu.
        /// Example usage: AddSubMenuItem("Construction", "Building", ...)
        /// </summary>
        /// <param name="_rootMenuName">The root menu name. E.g. Construction</param>
        /// <param name="_subMenuName">Name of the sub menu.</param>
        public void AddSubMenuItem(string _rootMenuName, string _subMenuName)
        {
            if (!subMenuMap.ContainsKey(_rootMenuName))
            {
                // No root menu matches given root menu name.
                return;
            }

            if (!subMenuMap[_rootMenuName].ContainsSubMenu(_subMenuName))
            {
                // No sub menu matches given sub menu name.
                return;
            }

            //TODO: Figure out how to pass data to let button display required information for each item.
            var button = Instantiate(toolbarSubMenuItemPrefab, toolbarSubMenuItemsParent.transform);
            var buttonController = button.GetComponent<ToolbarSubMenuItemButton>();

            subMenuMap[_rootMenuName].AddSubMenuItem(_subMenuName, buttonController);
        }
    }
}

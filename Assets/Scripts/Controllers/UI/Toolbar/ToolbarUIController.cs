using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class ToolbarUIController : MonoBehaviour
    {
        public static ToolbarUIController Instance { get; private set; }

        [SerializeField]
        private GameObject toolbarButtonPrefab = null;

        [SerializeField]
        private GameObject toolbarTabButtonPrefab = null;

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
        /// Maps the sub menus for the root toolbar menu names.
        /// </summary>
        private Dictionary<string, ToolbarSubMenu> subMenuMap = new Dictionary<string, ToolbarSubMenu>();

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
                    subMenuMap[_menuName].SetButtonsActive(false);
                }
                else
                {
                    if (subMenuMap.ContainsKey(currentSubMenu))
                    {
                        subMenuMap[currentSubMenu].SetButtonsActive(false);
                    }

                    currentSubMenu = _menuName;
                    subMenuMap[_menuName].SetButtonsActive(true);
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
                subMenuMap.Add(_rootMenu, new ToolbarSubMenu());
            }

            var button = Instantiate(toolbarTabButtonPrefab, toolbarSubMenuParent.transform);
            button.SetActive(false);
            var buttonController = button.GetComponent<ToolbarButtonConroller>();
            buttonController.SetButtonText(_subMenuName);
            buttonController.AddButtonClickAction(_clickAction);

            // TODO: Add button action (lambda) to display sub menu items to sub menu items panel when clicked.

            subMenuMap[_rootMenu].AddButton(buttonController);
        }

        /// <summary>
        /// Adds an item button for a specified sub menu.
        /// </summary>
        /// <param name="_subMenuName"></param>
        public void AddSubMenuItem(string _subMenuName)
        {
            if (!subMenuMap.ContainsKey(_subMenuName))
            {
                // No sub menu matches the given sub menu name.
                return;
            }

            // TODO: Add some kind of ISubMenuItem interface to create a button to add to the submenu. Needs new prefab + controller.
        }
    }
}

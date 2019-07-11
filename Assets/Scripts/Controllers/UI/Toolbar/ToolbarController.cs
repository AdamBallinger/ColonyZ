using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class ToolbarController : MonoBehaviour
    {
        /*[SerializeField]
        private GameObject toolbarButtonPrefab;

        [SerializeField]
        private GameObject toolbarTabButtonPrefab;

        [SerializeField]
        private GameObject toolbarSubMenuItemPrefab;

        /// <summary>
        /// Reference to the gameobject that parents all of the toolbar menu buttons. E.g. Construction, Commands, etc.
        /// </summary>
        [SerializeField]
        private GameObject toolBarButtonsParent;

        /// <summary>
        /// Reference to the gameobject that parents all the sub menu buttons. E.g For construction: Structures, Areas, etc.
        /// </summary>
        [SerializeField]
        private GameObject toolbarSubMenuParent;

        /// <summary>
        /// Reference to the gameobject that parents all the sub menu item buttons. E.g. construction -> building -> Wood Wall, Steel Wall etc.
        /// </summary>
        [SerializeField]
        private GameObject toolbarSubMenuItemsParent;

        /// <summary>
        /// Maps each root menu to its sub menu container.
        /// </summary>
        private Dictionary<string, ToolbarSubMenuContainer> subMenuMap = new Dictionary<string, ToolbarSubMenuContainer>();

        /// <summary>
        /// The current root menu that is open. Empty is no open root menu.
        /// </summary>
        private string currentRootMenu = string.Empty;

        private string currentSubMenu = string.Empty;*/

        private void Start()
        {
            /*AddRootMenu("Construction");
            AddRootMenu("Commands");
            AddRootMenu("Menu");

            AddSubMenu("Construction", "Building");
            AddSubMenu("Construction", "Area");

            AddSubMenu("Commands", "Work");

            AddSubMenu("Menu", "Exit", Application.Quit);
            AddSubMenu("Menu", "Dev");*/

            // TODO: Rewrite this utter garbage system to actually use TileObject SO references instead..
            // Maybe add a icon reference to each SpriteData so it can be easily accessed? <- Done
            
            /*AddSubMenuItem("Construction", "Building", "Wood Wall",
                           SpriteCache.GetSprite("Wood_Wall", 47), () =>
            {
                MouseController.Instance.BuildModeController.StartObjectBuild("Wood_Wall");
            });

            AddSubMenuItem("Construction", "Building", "Steel Wall", 
                           SpriteCache.GetSprite("Steel_Wall", 47), () =>
            {
                MouseController.Instance.BuildModeController.StartObjectBuild("Steel_Wall");
            });

            AddSubMenuItem("Construction", "Building", "Wooden Door", 
                           SpriteCache.GetSprite("Wood_Door", "closed_0"), () =>
            {
                MouseController.Instance.BuildModeController.StartObjectBuild("Wood_Door");
            });

            AddSubMenuItem("Construction", "Building", "Steel Door", 
                           SpriteCache.GetSprite("Steel_Door", "closed_0"), () =>
            {
                MouseController.Instance.BuildModeController.StartObjectBuild("Steel_Door");
            });

            AddSubMenuItem("Commands", "Work", "Demolish", 
                           SpriteCache.GetSprite("Overlay", "demolish"), () =>
            {
                MouseController.Instance.BuildModeController.StartDemolishBuild();
            });

            AddSubMenuItem("Commands", "Work", "Mine", 
                           SpriteCache.GetSprite("Overlay", "mine"), null);
            AddSubMenuItem("Commands", "Work", "Fell", 
                           SpriteCache.GetSprite("Overlay", "chop"), null);
            AddSubMenuItem("Commands", "Work", "Cancel", 
                           SpriteCache.GetSprite("Overlay", "cancel"), null);
            
            AddSubMenuItem("Menu", "Dev", "Tile Nodes", null, DevToolManager.Instance.ToggleTileNodes);*/
        }

        /*/// <summary>
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
                // If the menu is already open then disable all of its buttons.s
                if (currentRootMenu.Equals(_menuName))
                {
                    subMenuMap[currentRootMenu].SetMenuButtonsState(false);
                    // Disable any sub menu item buttons if they are enabled as this root menu is closing.
                    subMenuMap[_menuName].SetMenuItemButtonsState(currentSubMenu, false);

                    currentRootMenu = string.Empty;
                    currentSubMenu = string.Empty;

                    MouseController.Instance.Mode = MouseMode.Select;
                }
                else
                {
                    // Check that the current root menu exists in the sub menu map (Used to check for string.Empty)
                    if (subMenuMap.ContainsKey(currentRootMenu))
                    {
                        // Disable all the current sub menu buttons as a new root menu needs to open.
                        subMenuMap[currentRootMenu].SetMenuButtonsState(false);
                        // Also disable any open sub menu item button that were active.
                        subMenuMap[currentRootMenu].SetMenuItemButtonsState(currentSubMenu, false);
                        currentSubMenu = string.Empty;

                        MouseController.Instance.Mode = MouseMode.Select;
                    }

                    // Update the current root menu to the new menu name, and activate its buttons.
                    currentRootMenu = _menuName;
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

            // Add function to display sub menu items when button is clicked.
            buttonController.AddButtonClickAction(() =>
            {
                // Only do something if the sub menu has items.
                if (subMenuMap[_rootMenu].GetSubMenuItemCount(_subMenuName) > 0)
                {
                    // Remove the items if this sub menu is the one already open.
                    if (currentSubMenu.Equals(_subMenuName))
                    {
                        currentSubMenu = string.Empty;
                        subMenuMap[_rootMenu].SetMenuItemButtonsState(_subMenuName, false);
                    }
                    else
                    {
                        if (subMenuMap[_rootMenu].ContainsSubMenu(currentSubMenu))
                        {
                            subMenuMap[_rootMenu].SetMenuItemButtonsState(currentSubMenu, false);
                        }

                        currentSubMenu = _subMenuName;
                        subMenuMap[_rootMenu].SetMenuItemButtonsState(_subMenuName, true);
                    }
                }
            });

            subMenuMap[_rootMenu].AddButton(_subMenuName, buttonController);
        }

        /// <summary>
        /// Adds an item button for a specified sub menu.
        /// Example usage: AddSubMenuItem("Construction", "Building", ...)
        /// </summary>
        /// <param name="_rootMenuName">The root menu name. E.g. Construction</param>
        /// <param name="_subMenuName">Name of the sub menu.</param>
        /// <param name="_itemName"></param>
        /// <param name="_itemIcon"></param>
        /// <param name="_clickAction"></param>
        public void AddSubMenuItem(string _rootMenuName, string _subMenuName, string _itemName, Sprite _itemIcon, Action _clickAction)
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

            var button = Instantiate(toolbarSubMenuItemPrefab, toolbarSubMenuItemsParent.transform);
            button.SetActive(false);
            var buttonController = button.GetComponent<ToolbarSubMenuItemButton>();
            buttonController.SetButtonName(_itemName);
            buttonController.SetIcon(_itemIcon);
            buttonController.AddButtonClickAction(_clickAction);

            subMenuMap[_rootMenuName].AddSubMenuItem(_subMenuName, buttonController);
        }*/
    }
}

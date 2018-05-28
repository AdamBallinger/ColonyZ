using System.Collections.Generic;

namespace Controllers.UI.Toolbar
{
    public class ToolbarSubMenu
    {
        private List<ToolbarButtonConroller> Buttons { get; }

        private Dictionary<string, List<ToolbarButtonConroller>> Items { get; }

        public ToolbarSubMenu()
        {
            Buttons = new List<ToolbarButtonConroller>();
            Items = new Dictionary<string, List<ToolbarButtonConroller>>();
        }

        public void AddButton(ToolbarButtonConroller _button)
        {
            Buttons.Add(_button);
        }

        /// <summary>
        /// TODO: Add some kind of ISubMenuItem interface so the button can display information about the item being added
        /// </summary>
        /// <param name="_subMenuName"></param>
        public void AddSubMenuItem(string _subMenuName)
        {
            // TODO: Will need a new button controller and prefab for sub menu items.
        }

        public void SetButtonsActive(bool _activeState)
        {
            foreach(var button in Buttons)
            {
                button.gameObject.SetActive(_activeState);
            }
        }
    }
}

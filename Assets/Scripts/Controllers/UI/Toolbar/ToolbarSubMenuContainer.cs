using System.Collections.Generic;

namespace Controllers.UI.Toolbar
{
    public class ToolbarSubMenuContainer
    {
        /// <summary>
        /// Maps each sub menu to its button.
        /// </summary>
        private Dictionary<string, ToolbarButtonConroller> Buttons { get; }

        /// <summary>
        /// Maps each sub menu to its items.
        /// </summary>
        private Dictionary<string, List<ToolbarSubMenuItemButton>> Items { get; }

        public ToolbarSubMenuContainer()
        {
            Buttons = new Dictionary<string, ToolbarButtonConroller>();
            Items = new Dictionary<string, List<ToolbarSubMenuItemButton>>();
        }

        /// <summary>
        /// Adds a new menu button for this sub menu container.
        /// </summary>
        /// <param name="_subMenuName"></param>
        /// <param name="_button"></param>
        public void AddButton(string _subMenuName, ToolbarButtonConroller _button)
        {
            if(!Buttons.ContainsKey(_subMenuName))
            {
                Buttons.Add(_subMenuName, _button);
            }
        }

        /// <summary>
        /// Adds an item to a specified sub menu button for this sub menu container.
        /// </summary>
        /// <param name="_subMenuName"></param>
        /// <param name="_button"></param>
        public void AddSubMenuItem(string _subMenuName, ToolbarSubMenuItemButton _button)
        {
            if(Buttons.ContainsKey(_subMenuName))
            {
                // No sub menu button matches the provided sub menu name.
                return;
            }

            if(!Items.ContainsKey(_subMenuName))
            {
                Items.Add(_subMenuName, new List<ToolbarSubMenuItemButton>());
            }

            Items[_subMenuName].Add(_button);
        }

        /// <summary>
        /// Sets the active state for this sub menu container sub menu buttons.
        /// </summary>
        /// <param name="_activeState"></param>
        public void SetMenuButtonsState(bool _activeState)
        {
            foreach(var pair in Buttons)
            {
                pair.Value.gameObject.SetActive(_activeState);
            }
        }
    }
}

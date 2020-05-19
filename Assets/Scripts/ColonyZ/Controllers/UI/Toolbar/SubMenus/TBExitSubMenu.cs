using UnityEngine;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBExitSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            // TODO: Save game, then switch to menu when there is one.
            AddItem("Menu", null, null);

            // TODO: Save game before quitting to desktop.
            AddItem("Desktop", null, Application.Quit);
        }
    }
}
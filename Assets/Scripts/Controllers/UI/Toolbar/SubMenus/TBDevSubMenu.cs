﻿using Controllers.Dev;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBDevSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Tile Nodes", null, () => DevToolManager.Instance.ToggleTileNodes());
        }

        protected override void OnDisabled()
        {
            DevToolManager.Instance.DisableTileNodes();
        }
    }
}
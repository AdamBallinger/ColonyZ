﻿using Models.Sprites;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBWorkSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Destroy", SpriteCache.GetSprite("Overlay", 0), 
                    () => MouseController.Instance.BuildModeController.SetDemolishMode());
        }
    }
}
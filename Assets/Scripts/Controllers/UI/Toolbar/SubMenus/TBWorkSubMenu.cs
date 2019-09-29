using Models.Sprites;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBWorkSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Demolish", SpriteCache.GetSprite("Overlay", 0), 
                    () => MouseController.Instance.BuildModeController.SetDemolishMode());
            
            AddItem("Fell", SpriteCache.GetSprite("Overlay", 2),
                    () => MouseController.Instance.BuildModeController.SetHarvestMode());
        }
    }
}

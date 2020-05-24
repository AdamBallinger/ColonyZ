using ColonyZ.Models.Sprites;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBWorkSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Demolish", SpriteCache.GetSprite("Overlay", 0),
                () => MouseController.Instance.BuildModeController.SetDemolishMode());

            AddItem("Mine", SpriteCache.GetSprite("Overlay", 1),
                () => MouseController.Instance.BuildModeController.SetMineMode());

            AddItem("Fell", SpriteCache.GetSprite("Overlay", 2),
                () => MouseController.Instance.BuildModeController.SetFellMode());

            AddItem("Cancel", SpriteCache.GetSprite("Overlay", 3),
                () => MouseController.Instance.BuildModeController.SetCancelMode());
        }
    }
}
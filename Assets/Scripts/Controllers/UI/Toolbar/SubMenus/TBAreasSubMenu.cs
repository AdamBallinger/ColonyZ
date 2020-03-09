using Models.Map.Areas;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBAreasSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Stockpile", null, () => MouseController.Instance.BuildModeController.SetAreaMode(AreaType.Stockpile));
        }
    }
}
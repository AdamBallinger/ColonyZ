using Models.Map.Zones;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBZonesSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Storage", null,
                () => MouseController.Instance.BuildModeController.SetZone(new StorageZone()));
        }
    }
}
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Zones;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBZonesSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Storage", null,
                () => World.Instance.WorldActionProcessor.SetZone(new StorageZone()));
        }
    }
}
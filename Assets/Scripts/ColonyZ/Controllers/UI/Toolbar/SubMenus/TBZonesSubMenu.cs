using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Models.Sprites;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBZonesSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Delete", SpriteCache.GetSprite("Toolbar_Zones", 0),
                () => World.Instance.WorldActionProcessor.SetZoneDeleteMode());
            AddItem("Expand", SpriteCache.GetSprite("Toolbar_Zones", 2),
                () => World.Instance.WorldActionProcessor.SetZoneExpandMode());
            AddItem("Shrink", SpriteCache.GetSprite("Toolbar_Zones", 3),
                () => World.Instance.WorldActionProcessor.SetZoneShrinkMode());
            AddItem("Storage", SpriteCache.GetSprite("Toolbar_Zones", 1),
                () => World.Instance.WorldActionProcessor.SetZoneCreateMode(new StorageZone()));
        }
    }
}
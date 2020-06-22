using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles.Objects;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBObjectsSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            foreach (var obj in TileObjectCache.TileObjects)
            {
                if (!obj.Buildable) continue;

                AddItem(obj.ObjectName, obj.GetIcon(),
                    () => World.Instance.WorldActionProcessor.SetBuildMode(obj));
            }
        }
    }
}
using Models.Map.Tiles.Objects;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBObjectsSubMenu : TBSubMenu
    {        
        protected override void OnEnabled()
        {
            foreach(var to in TileObjectCache.TileObjects)
            {
                AddItem(to.ObjectName, to.GetIcon(), () => MouseController.Instance.BuildModeController.StartObjectBuild(to));
            }
        }
        
        protected override void OnDisabled()
        {
            MouseController.Instance.Mode = MouseMode.Select;
        }
    }
}

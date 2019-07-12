using Models.Map.Tiles.Objects;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBObjectsSubMenu : TBSubMenu
    {
        // TODO: Remove this and access all TileObjects from a cache.
        public TileObject[] objects;
        
        protected override void OnEnabled()
        {
            foreach(var to in objects)
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

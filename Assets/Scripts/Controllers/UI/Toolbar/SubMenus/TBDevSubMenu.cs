using Controllers.Dev;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBDevSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Tile Nodes", null, () => DevToolManager.Instance.ToggleTileNodes());
            AddItem("Item Spawner", null, () => DevToolManager.Instance.ToggleItemTool());
            AddItem("Path Count", null, () => DevToolManager.Instance.TogglePathCount());
            AddItem("Jobs Info", null, () => DevToolManager.Instance.ToggleJobsInfo());
            AddItem("God Mode", null, () =>
                MouseController.Instance.BuildModeController.GodMode =
                    !MouseController.Instance.BuildModeController.GodMode);
            AddItem("Rooms Debug", null, () => DevToolManager.Instance.ToggleRoomsDebug());
        }

        protected override void OnDisabled()
        {
            DevToolManager.Instance.DisableTileNodes();
        }
    }
}
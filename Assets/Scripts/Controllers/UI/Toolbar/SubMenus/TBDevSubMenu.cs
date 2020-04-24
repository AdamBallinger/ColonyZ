using Controllers.Dev;

namespace Controllers.UI.Toolbar.SubMenus
{
    public class TBDevSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Tile Nodes", null, () => DevToolManager.Instance.ToggleTileNodes());
            AddItem("Item Spawner", null, () => DevToolManager.Instance.ToggleItemTool());
            AddItem("Path Debug", null, () => DevToolManager.Instance.TogglePathDebug());
            AddItem("Jobs Info", null, () => DevToolManager.Instance.ToggleJobsInfo());
            AddItem("God Mode", null, () =>
                MouseController.Instance.BuildModeController.GodMode =
                    !MouseController.Instance.BuildModeController.GodMode);
            AddItem("Areas Debug", null, () => DevToolManager.Instance.ToggleAreasDebug());
            AddItem("Regions Debug", null, () => DevToolManager.Instance.ToggleRegionsDebug());
        }

        protected override void OnDisabled()
        {
            DevToolManager.Instance.DisableTileNodes();
        }
    }
}
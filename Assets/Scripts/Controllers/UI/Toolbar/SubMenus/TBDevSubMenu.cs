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
        }

        protected override void OnDisabled()
        {
            DevToolManager.Instance.DisableTileNodes();
        }
    }
}
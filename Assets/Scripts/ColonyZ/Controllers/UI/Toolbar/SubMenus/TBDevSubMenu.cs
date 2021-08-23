using ColonyZ.Controllers.Dev;
using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBDevSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Tile Nodes", SpriteCache.GetSprite("UI", 2),
                () => DevToolManager.Instance.ToggleTileNodes());
            AddItem("Path Debug", SpriteCache.GetSprite("UI", 3),
                () => DevToolManager.Instance.TogglePathDebug());
            AddItem("Areas Overlay", SpriteCache.GetSprite("UI", 4),
                () => DevToolManager.Instance.ToggleAreasDebug());
            AddItem("Regions Overlay", SpriteCache.GetSprite("UI", 5),
                () => DevToolManager.Instance.ToggleRegionsDebug());
            AddItem("Item Spawner", SpriteCache.GetSprite("UI", 6),
                () => DevToolManager.Instance.ToggleItemTool());
            AddItem("Jobs Info", SpriteCache.GetSprite("UI", 7),
                () => DevToolManager.Instance.ToggleJobsInfo());
            AddItem("God Mode", SpriteCache.GetSprite("UI", 8),
                () => World.Instance.WorldActionProcessor.ToggleGodMode());
        }
    }
}
using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using ColonyZ.Models.UI;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBWorkSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Demolish", SpriteCache.GetSprite("Overlay", (byte)OverlayType.Hammer),
                () => World.Instance.WorldActionProcessor.SetDemolishMode());

            AddItem("Mine", SpriteCache.GetSprite("Overlay", (byte)OverlayType.Pickaxe),
                () => World.Instance.WorldActionProcessor.SetGatherMode(GatherMode.Mine));

            AddItem("Fell", SpriteCache.GetSprite("Overlay", (byte)OverlayType.Axe),
                () => World.Instance.WorldActionProcessor.SetGatherMode(GatherMode.Fell));

            AddItem("Cancel", SpriteCache.GetSprite("Overlay", (byte)OverlayType.Cancel),
                () => World.Instance.WorldActionProcessor.SetCancelMode());
        }
    }
}
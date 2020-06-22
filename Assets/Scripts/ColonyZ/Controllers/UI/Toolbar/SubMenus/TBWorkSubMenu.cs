using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBWorkSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Demolish", SpriteCache.GetSprite("Overlay", 0),
                () => World.Instance.WorldActionProcessor.SetDemolishMode());

            AddItem("Mine", SpriteCache.GetSprite("Overlay", 1),
                () => World.Instance.WorldActionProcessor.SetGatherMode(GatherMode.Mine));

            AddItem("Fell", SpriteCache.GetSprite("Overlay", 2),
                () => World.Instance.WorldActionProcessor.SetGatherMode(GatherMode.Fell));

            AddItem("Cancel", SpriteCache.GetSprite("Overlay", 3),
                () => World.Instance.WorldActionProcessor.SetCancelMode());
        }
    }
}
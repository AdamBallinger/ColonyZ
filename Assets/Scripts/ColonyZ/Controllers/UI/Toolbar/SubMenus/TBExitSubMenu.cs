using ColonyZ.Models.Sprites;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBExitSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Menu", SpriteCache.GetSprite("UI", 0), () => SceneManager.LoadScene("_menu_"));
            AddItem("Desktop", SpriteCache.GetSprite("UI", 1), Application.Quit);
        }
    }
}
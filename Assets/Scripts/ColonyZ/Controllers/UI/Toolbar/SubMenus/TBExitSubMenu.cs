using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColonyZ.Controllers.UI.Toolbar.SubMenus
{
    public class TBExitSubMenu : TBSubMenu
    {
        protected override void OnEnabled()
        {
            AddItem("Menu", null, () => SceneManager.LoadScene("_menu_"));
            AddItem("Desktop", null, Application.Quit);
        }
    }
}
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class TBController : MonoBehaviour
    {
        private TBMenuController activeMenu;

        public void OpenMenu(TBMenuController _menu)
        {
            if (_menu == null)
            {
                activeMenu?.Disable();
                activeMenu = null;
                return;
            }
            
            if (activeMenu == null)
            {
                activeMenu = _menu;
                activeMenu.Enable();
                return;
            }
            
            if (activeMenu == _menu)
            {
                _menu.Disable();
                activeMenu = null;
                return;
            }
            
            activeMenu.Disable();
            activeMenu = _menu;
            activeMenu.Enable();
        }
    }
}

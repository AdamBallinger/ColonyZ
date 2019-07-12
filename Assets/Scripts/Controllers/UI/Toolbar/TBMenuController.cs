using Controllers.UI.Toolbar.SubMenus;
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class TBMenuController : MonoBehaviour
    {
        private TBSubMenu openSubMenu;
        
        public void Enable()
        {
            openSubMenu?.Enable();
            gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            openSubMenu?.Disable();
            gameObject.SetActive(false);
        }
        
        public void SetOpenMenu(TBSubMenu _menu)
        {
            openSubMenu = _menu;
        }
    }
}

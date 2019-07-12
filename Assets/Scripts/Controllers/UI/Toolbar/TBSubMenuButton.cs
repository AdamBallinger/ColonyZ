using Controllers.UI.Toolbar.SubMenus;
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class TBSubMenuButton : MonoBehaviour
    {
        [SerializeField]
        private TBSubMenu subMenu;
        
        public void OnClick()
        {
            if(subMenu.Open)
            {
                subMenu.Disable();
                return;
            }
            
            subMenu.Enable();
        }
    }
}

using Controllers.UI.Toolbar.SubMenus;
using UnityEngine;

namespace Controllers.UI.Toolbar
{
    public class TBMenuController : MonoBehaviour
    {
        public GameObject SubMenuItemPrefab => subMenuItemButtonPrefab;

        public GameObject SubMenuItemContainer => subMenuItemContainer;
        
        [SerializeField]
        private GameObject subMenuItemButtonPrefab;

        [SerializeField]
        private GameObject subMenuItemContainer;
        
        private TBSubMenu openSubMenu;
        
        public void Enable()
        {
            SetOpenMenu(null);
            gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            SetOpenMenu(null);
            gameObject.SetActive(false);
        }
        
        public void SetOpenMenu(TBSubMenu _menu)
        {
            if (_menu == null)
            {
                CloseSubMenu();
                return;
            }
            
            if (_menu == openSubMenu)
            {
                CloseSubMenu();
                return;
            }
            
            CloseSubMenu();
            OpenSubMenu(_menu);
        }
        
        private void OpenSubMenu(TBSubMenu _menu)
        {
            openSubMenu = _menu;
            openSubMenu?.Enable();
        }
        
        private void CloseSubMenu()
        {
            openSubMenu?.Disable();
            openSubMenu = null;
        }
    }
}

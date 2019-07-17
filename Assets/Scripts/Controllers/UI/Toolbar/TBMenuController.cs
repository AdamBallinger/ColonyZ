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
            openSubMenu?.Disable();
            openSubMenu = _menu;
        }
    }
}

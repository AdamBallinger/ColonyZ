using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.UI.Toolbar.SubMenus
{
    public abstract class TBSubMenu : MonoBehaviour
    {
        public bool Open { get; protected set; }

        private List<GameObject> itemButtons;

        private TBMenuController menuController;
        
        private void Start()
        {
            menuController = GetComponentInParent<TBMenuController>();
            itemButtons = new List<GameObject>();
        }
        
        protected void AddItem(string _buttonText, Sprite _buttonIcon, Action _onClick)
        {
            var button = Instantiate(menuController.SubMenuItemPrefab, menuController.SubMenuItemContainer.transform);
            button.GetComponent<TBSubMenuItemButton>().Set(_buttonText, _buttonIcon, _onClick);
            itemButtons.Add(button);
        }
        
        public void Enable()
        {
            Open = true;
            menuController.SubMenuItemContainer.SetActive(true);
            OnEnabled();
        }
        
        public void Disable()
        {
            ClearContainer();
            Open = false;
            menuController.SubMenuItemContainer.SetActive(false);
            OnDisabled();
        }
        
        public void OnMenuClick()
        {
            menuController.SetOpenMenu(this);
        }
        
        private void ClearContainer()
        {
            foreach(var button in itemButtons)
            {
                Destroy(button);
            }
            
            itemButtons.Clear();
        }

        protected abstract void OnEnabled();
        
        protected virtual void OnDisabled()
        {
            MouseController.Instance.Mode = MouseMode.Select;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers.UI.Toolbar.SubMenus
{
    public abstract class TBSubMenu : MonoBehaviour
    {
        public bool Open { get; protected set; }
        
        [SerializeField]
        private GameObject buttonPrefab;
        
        [SerializeField, Tooltip("The container gameobject for this submenus items.")]
        private GameObject buttonsContainer;

        private List<GameObject> itemButtons;

        private TBMenuController menuController;
        
        private void Start()
        {
            menuController = GetComponentInParent<TBMenuController>();
            itemButtons = new List<GameObject>();
        }
        
        protected void AddItem(string _buttonText, Sprite _buttonIcon, Action _onClick)
        {
            var button = Instantiate(buttonPrefab, buttonsContainer.transform);
            button.GetComponent<TBSubMenuItemButton>().Set(_buttonText, _buttonIcon, _onClick);
            itemButtons.Add(button);
        }
        
        public void Enable()
        {
            menuController.SetOpenMenu(this);
            Open = true;
            buttonsContainer.SetActive(true);
            OnEnabled();
        }
        
        public void Disable()
        {
            ClearContainer();
            menuController.SetOpenMenu(null);
            Open = false;
            buttonsContainer.SetActive(false);
            OnDisabled();
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

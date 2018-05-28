using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI.Toolbar
{
    public class ToolbarSubMenuItemButton : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI buttonText = null;

        [SerializeField]
        private Image buttonIcon = null;

        private Action buttonClick;

        public void SetIcon(Sprite _sprite)
        {
            buttonIcon.sprite = _sprite;
        }

        public void SetButtonName(string _name)
        {
            buttonText.text = _name;
        }

        public void AddButtonClickAction(Action _action)
        {
            if(_action == null)
            {
                return;
            }

            buttonClick += _action;
        }

        public void OnButtonClick()
        {
            buttonClick?.Invoke();
        }
    }
}

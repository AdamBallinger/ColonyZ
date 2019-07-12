using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI.Toolbar
{
    public class TBSubMenuItemButton : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI buttonText;

        [SerializeField]
        private Image buttonIcon;

        private Action onClick;
        
        public void Set(string _text, Sprite _icon, Action _onClick)
        {
            buttonText.text = _text;
            buttonIcon.sprite = _icon;
            onClick += _onClick;
        }
        
        public void OnClick()
        {
            onClick?.Invoke();
        }
    }
}

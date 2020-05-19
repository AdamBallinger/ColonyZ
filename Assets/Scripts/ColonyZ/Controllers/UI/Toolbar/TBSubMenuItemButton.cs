using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI.Toolbar
{
    public class TBSubMenuItemButton : MonoBehaviour
    {
        [SerializeField] private Image buttonIcon;

        [SerializeField] private TextMeshProUGUI buttonText;

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
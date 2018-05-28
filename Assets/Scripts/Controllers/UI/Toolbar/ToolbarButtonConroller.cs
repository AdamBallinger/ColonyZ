using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers.UI.Toolbar
{
    public class ToolbarButtonConroller : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI buttonText = null;

        [SerializeField]
        private Button button = null;

        private Action buttonClick;

        /// <summary>
        /// Controls the interactability of the button.
        /// </summary>
        /// <param name="_state"></param>
        public void SetButtonState(bool _state)
        {
            button.interactable = _state;
        }

        /// <summary>
        /// Sets the text for the button.
        /// </summary>
        /// <param name="_text"></param>
        public void SetButtonText(string _text)
        {
            buttonText.text = _text;
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

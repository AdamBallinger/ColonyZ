using ColonyZ.Models.UI.Context;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColonyZ.Controllers.UI.Context
{
    public class ContextMenuButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        public void Set(ContextAction _action)
        {
            buttonText.text = _action.Name;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _action.Action.Invoke());
        }
    }
}
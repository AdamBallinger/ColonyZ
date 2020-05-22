using System;
using ColonyZ.Models.Map;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.UI.MainMenu
{
    public class WorldSizeButtonController : MonoBehaviour
    {
        [SerializeField] private TMP_Text buttonName;
        [SerializeField] private TMP_Text buttonLabel;

        private WorldSize size;
        private Action<WorldSize> onClick;

        public void SetSize(WorldSize _size, Action<WorldSize> _onClick)
        {
            size = _size;
            buttonName.text = size.Name;
            buttonLabel.text = size.ToString();
            onClick = _onClick;
        }

        public void OnClick()
        {
            onClick?.Invoke(size);
        }
    }
}
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

        private WorldSizeTypes.WorldSize size;
        private Action<WorldSizeTypes.WorldSize> onClick;

        public void SetSize(WorldSizeTypes.WorldSize _size, Action<WorldSizeTypes.WorldSize> _onClick)
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
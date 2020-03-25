using Models.Map.Areas;
using TMPro;
using UnityEngine;

namespace Controllers.UI
{
    public class AreaOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text areaNameText;

        [SerializeField] private RectTransform rTransform;

        private Area assignedArea;

        public void Set(Area _area)
        {
            assignedArea = _area;
            areaNameText.text = _area.AreaName;
            rTransform.anchoredPosition = _area.Origin - Vector2.one * 0.5f;
            rTransform.sizeDelta = _area.Size;
        }
    }
}
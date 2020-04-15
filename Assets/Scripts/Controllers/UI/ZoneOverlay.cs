using Models.Map.Zones;
using TMPro;
using UnityEngine;

namespace Controllers.UI
{
    public class ZoneOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text areaNameText;

        [SerializeField] private RectTransform rTransform;

        private Zone assignedZone;

        public void Set(Zone _zone)
        {
            assignedZone = _zone;
            areaNameText.text = _zone.ZoneName;
            rTransform.anchoredPosition = _zone.Origin - Vector2.one * 0.5f;
            rTransform.sizeDelta = _zone.Size;
        }
    }
}
using ColonyZ.Models.Map.Zones;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.UI
{
    public class ZoneOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text areaNameText;

        private Zone assignedZone;

        [SerializeField] private RectTransform rTransform;

        public void Set(Zone _zone)
        {
            assignedZone = _zone;
            areaNameText.text = _zone.ZoneName;
            rTransform.anchoredPosition = _zone.Origin - Vector2.one * 0.5f;
            rTransform.sizeDelta = _zone.Size;
        }
    }
}
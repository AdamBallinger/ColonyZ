using System.Collections.Generic;
using ColonyZ.Models.Map.Zones;
using UnityEngine;

namespace ColonyZ.Controllers.UI
{
    public class ZoneOverlayManager : MonoBehaviour
    {
        private Dictionary<Zone, GameObject> overlayMap;
        [SerializeField] private GameObject overlayPrefab;

        public void Init()
        {
            overlayMap = new Dictionary<Zone, GameObject>();
            ZoneManager.Instance.zoneCreatedEvent += CreateOverlay;
            ZoneManager.Instance.zoneDeletedEvent += DeleteOverlay;
        }

        private void CreateOverlay(Zone _zone)
        {
            var overlay = Instantiate(overlayPrefab).GetComponent<ZoneOverlay>();
            overlay.Set(_zone);

            overlayMap.Add(_zone, overlay.gameObject);
        }

        private void DeleteOverlay(Zone _zone)
        {
            var overlay = overlayMap[_zone];
            overlayMap.Remove(_zone);
            Destroy(overlay);
        }
    }
}
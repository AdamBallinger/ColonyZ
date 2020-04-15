using System.Collections.Generic;
using Models.Map.Zones;
using UnityEngine;

namespace Controllers.UI
{
    public class ZoneOverlayManager : MonoBehaviour
    {
        [SerializeField] private GameObject overlayPrefab;

        private Dictionary<Zone, GameObject> overlayMap;

        private void Start()
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
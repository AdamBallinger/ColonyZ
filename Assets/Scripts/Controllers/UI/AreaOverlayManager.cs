using System.Collections.Generic;
using Models.Map.Areas;
using UnityEngine;

namespace Controllers.UI
{
    public class AreaOverlayManager : MonoBehaviour
    {
        [SerializeField] private GameObject overlayPrefab;

        private Dictionary<Area, GameObject> overlayMap;

        private void Start()
        {
            overlayMap = new Dictionary<Area, GameObject>();
            AreaManager.Instance.onAreaCreatedEvent += CreateOverlay;
            AreaManager.Instance.onAreaDeletedEvent += DeleteOverlay;
        }

        private void CreateOverlay(Area _area)
        {
            var overlay = Instantiate(overlayPrefab).GetComponent<AreaOverlay>();
            overlay.Set(_area);

            overlayMap.Add(_area, overlay.gameObject);
        }

        private void DeleteOverlay(Area _area)
        {
            var overlay = overlayMap[_area];
            overlayMap.Remove(_area);
            Destroy(overlay);
        }
    }
}
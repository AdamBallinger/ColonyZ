using System.Collections.Generic;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Models.Map.Zones;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.UI
{
    public class ZoneOverlayManager : MonoBehaviour
    {
        [SerializeField] private MeshFilter overlayMeshFilter;
        [SerializeField] private MeshFilter selectedMeshFilter;
        
        private Dictionary<Zone, Mesh> overlayMap;

        private Mesh overlayMesh;
        private List<Color> overlayColors;

        public void Init()
        {
            overlayMap = new Dictionary<Zone, Mesh>();
            ZoneManager.Instance.zoneCreatedEvent += ZoneCreated;
            ZoneManager.Instance.zoneUpdatedEvent += ZoneUpdated;
            ZoneManager.Instance.zoneDeletedEvent += ZoneDeleted;
            World.Instance.tileChangedEvent += UpdateGlobalOverlayColors;

            overlayColors = new List<Color>();
            overlayMesh = MeshUtils.CreateMesh("Zone Global Overlay", World.Instance.GetTiles(), new Color(0, 0, 0, 0));
            overlayMeshFilter.mesh = overlayMesh;
        }

        private void ZoneCreated(Zone _zone)
        {
            if (overlayMap.ContainsKey(_zone))
            {
                Debug.LogWarning("[ZoneOverlayManager] Created zone already exists in zone map.");
                return;
            }
            
            overlayMap.Add(_zone, MeshUtils.CreateMesh(_zone.ZoneName, _zone.Tiles, new Color(1, 1, 1, 0.3f)));
        }

        private void ZoneUpdated(Zone _zone)
        {
            if (!overlayMap.ContainsKey(_zone))
            {
                Debug.LogWarning("[ZoneOverlayManager] Tried to update a zone that doesn't exist in zone map.");
                return;
            }

            overlayMap[_zone] = MeshUtils.CreateMesh(_zone.ZoneName, _zone.Tiles, new Color(1, 1, 1, 0.3f));
        }

        private void ZoneDeleted(Zone _zone)
        {
            if (!overlayMap.ContainsKey(_zone))
            {
                Debug.LogWarning("[ZoneOverlayManager] Attempted to delete zone that doesn't exist in zone map.");
                return;
            }

            overlayMap.Remove(_zone);
            ZoneManager.Instance.CurrentZoneBeingModified = null;

            if (Equals(SelectionController.currentSelection, _zone))
            {
                SelectionController.currentSelection = null;
            }
        }

        private void UpdateGlobalOverlayColors(Tile _tile)
        {
            overlayMesh.GetColors(overlayColors);

            var index = World.Instance.GetTileIndex(_tile) * 4;
            overlayColors[index++] = _tile.Zone?.Color ?? Color.clear;
            overlayColors[index++] = _tile.Zone?.Color ?? Color.clear;
            overlayColors[index++] = _tile.Zone?.Color ?? Color.clear;
            overlayColors[index] = _tile.Zone?.Color ?? Color.clear;
            
            overlayMesh.SetColors(overlayColors);
        }

        private void Update()
        {
            if (!(SelectionController.currentSelection is Zone) &&
                MouseController.Instance.Mode == MouseMode.Select)
            {
                ZoneManager.Instance.CurrentZoneBeingModified = null;
            }

            if (ZoneManager.Instance.CurrentZoneBeingModified != null)
            {
                if (overlayMap.TryGetValue(ZoneManager.Instance.CurrentZoneBeingModified, out var mesh))
                {
                    selectedMeshFilter.mesh = mesh;
                }
            }
            else
            {
                selectedMeshFilter.mesh = null;
            }
        }
    }
}
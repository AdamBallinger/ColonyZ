using ColonyZ.Controllers.UI;
using ColonyZ.Models.Entities.Living;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace ColonyZ.Controllers.Dev
{
    public class PathDebugTool : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private GameObject pathDebugRoot;

        [SerializeField] private TMP_Text pathQueueText;
        [SerializeField] private TMP_Text pathTestText;
        
        private Tile pathStart, pathEnd;

        private void Start()
        {
            MouseController.Instance.mouseClickEvent += (_btn, _tile, _ui) =>
            {
                if (_btn == MouseButton.RightMouse) MouseClick(_tile, _ui);
            };

            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.red;

            pathDebugRoot.SetActive(enabled);
        }

        private void OnDestroy()
        {
            lineRenderer.positionCount = 0;
        }

        private void OnDisable()
        {
            pathQueueText.text = string.Empty;
            pathTestText.text = string.Empty;

            lineRenderer.positionCount = 0;
        }

        private void Update()
        {
            if (!enabled) return;
            pathQueueText.text = $"Queued Paths: {PathFinder.Instance.RequestCount.ToString()}\n" +
                                 $"Characters: {World.Instance.Characters.Count}";

            if (SelectionController.currentSelection is LivingEntity le)
            {
                if (le.Motor.path == null || le.Motor.path.IsValid == false) return;

                OnPath(le.Motor.path);
            }
        }

        public void Toggle()
        {
            enabled = !enabled;
            pathDebugRoot.SetActive(enabled);
            if (enabled) OnDisable();
        }

        private void MouseClick(Tile _tile, bool _ui)
        {
            if (_ui || !pathDebugRoot.activeSelf) return;
            if (pathStart != null && pathEnd != null)
            {
                pathStart = null;
                pathEnd = null;
            }

            if (pathStart == null)
            {
                pathStart = _tile;
            }
            else
            {
                pathEnd = _tile;
                RequestPath();
            }
        }

        private void RequestPath()
        {
            if (pathStart == null || pathEnd == null) return;
            PathFinder.NewRequest(pathStart, pathEnd, OnPath, false);
        }

        private void OnPath(Path _p)
        {
            if (_p == null || !_p.IsValid)
            {
                pathTestText.text = "Invalid path.";
                lineRenderer.positionCount = 0;
                return;
            }
            
            pathTestText.text = $"Compute time: {_p.ComputeTime}ms\n" +
                                $"Path length: {_p.SmoothSize}";

            lineRenderer.positionCount = _p.SmoothSize - _p.CurrentIndex;

            var vectors = new Vector3[lineRenderer.positionCount];

            for (var i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pos = _p.SmoothPath[i +_p.CurrentIndex];
                pos.z = 0.0f;
                vectors[i] = pos;
            }

            lineRenderer.SetPositions(vectors);
        }
    }
}
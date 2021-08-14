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

        private Tile pathStart, pathEnd;
        [SerializeField] private TMP_Text pathTestText;
        [SerializeField] private TMP_Text regionalStatusText;

        private void Start()
        {
            MouseController.Instance.mouseClickEvent += (_btn, _tile, _ui) =>
            {
                if (_btn == MouseButton.RightMouse) MouseClick(_tile, _ui);
            };
            lineRenderer.startWidth = 1.0f;
            lineRenderer.endWidth = 1.0f;
            lineRenderer.widthMultiplier = 0.2f;

            var state = PathFinder.Instance.UseRegionalPathfinding;
            regionalStatusText.text = state
                ? "Regional Pathing: <color=green>ON</color>"
                : "Regional Pathing: <color=red>OFF</color>";
            
            pathDebugRoot.SetActive(enabled);
        }

        private void OnDisable()
        {
            pathQueueText.text = string.Empty;
            pathTestText.text = string.Empty;

            lineRenderer.positionCount = 0;
        }

        private void Update()
        {
            pathQueueText.text = $"Queued Paths: {PathFinder.Instance.TaskCount.ToString()}\n" +
                                 $"Characters: {World.Instance.Characters.Count}";
        }

        public void ToggleRegionalPathing()
        {
            var state = PathFinder.Instance.UseRegionalPathfinding =
                !PathFinder.Instance.UseRegionalPathfinding;
            regionalStatusText.text = state
                ? "Regional Pathing: <color=green>ON</color>"
                : "Regional Pathing: <color=red>OFF</color>";
            RequestPath();
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
            PathFinder.NewRequest(pathStart, pathEnd, OnPath);
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

            lineRenderer.positionCount = _p.SmoothSize;

            var vectors = new Vector3[_p.SmoothSize];

            for (var i = 0; i < _p.SmoothSize; i++)
            {
                Vector3 pos = _p.SmoothPath[i];
                pos.z = 0.0f;
                vectors[i] = pos;
            }

            lineRenderer.SetPositions(vectors);
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.red;
        }

        private void OnDrawGizmos()
        {
            if (SelectionController.currentSelection is LivingEntity)
            {
                var le = SelectionController.currentSelection as LivingEntity;
                if (le?.Motor.path == null || le.Motor.path.IsValid == false) return;

                OnPath(le.Motor.path);

                var current = le.Motor.path.Current;

                // Display where the path started.
                Gizmos.color = Color.green;
                Gizmos.DrawCube(le.Motor.path.SmoothPath[0], Vector2.one * 0.5f);

                // Display where the motor is heading.
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(current, Vector2.one * 0.35f);

                // Display the tile the entity is currently considered on.
                Gizmos.color = Color.red;
                Gizmos.DrawCube(le.CurrentTile.Position, Vector2.one * 0.15f);
            }
        }
    }
}
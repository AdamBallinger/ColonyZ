﻿using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Models.Map.Tiles;
using TMPro;
using UnityEngine;

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
            MouseController.Instance.mouseClickEvent += MouseClick;
            lineRenderer.startWidth = 1.0f;
            lineRenderer.endWidth = 1.0f;
            lineRenderer.widthMultiplier = 0.2f;

            var state = PathFinder.Instance.UseRegionalPathfinding;
            regionalStatusText.text = state
                ? "Regional Pathing: <color=green>ON</color>"
                : "Regional Pathing: <color=red>OFF</color>";
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
            pathDebugRoot.SetActive(!pathDebugRoot.activeSelf);
            if (!pathDebugRoot.activeSelf) OnDisable();
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
            if (!_p.IsValid)
            {
                pathTestText.text = "Invalid path.";
                lineRenderer.positionCount = 0;
                return;
            }

            pathTestText.text = $"Compute time: {_p.ComputeTime}ms\n" +
                                $"Path length: {_p.TilePath.Count}";

            lineRenderer.positionCount = _p.TilePath.Count;

            var vectors = new Vector3[_p.TilePath.Count];

            for (var i = 0; i < _p.TilePath.Count; i++)
            {
                Vector3 pos = _p.TilePath[i].Position;
                pos.z = 0.0f;
                vectors[i] = pos;
            }

            lineRenderer.SetPositions(vectors);
        }
    }
}
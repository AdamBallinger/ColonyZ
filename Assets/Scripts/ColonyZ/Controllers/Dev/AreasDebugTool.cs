using System.Collections.Generic;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Areas;
using ColonyZ.Utils;
using TMPro;
using UnityEngine;

namespace ColonyZ.Controllers.Dev
{
    public class AreasDebugTool : MonoBehaviour
    {
        private readonly Dictionary<int, Color> areaColorMap = new Dictionary<int, Color>();

        [SerializeField] private TMP_Text areasText;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] [Range(0.0f, 1.0f)] private float overlayAlpha = 0.25f;

        private Mesh tileMesh;

        private List<Color> colors = new List<Color>();

        private void Start()
        {
            AreaManager.Instance.areasUpdatedEvent += UpdateOverlay;
            
            GenerateTileMesh();
            UpdateOverlay();
        }
        
        private void UpdateOverlay()
        {
            if (!enabled)
            {
                areasText.text = string.Empty;
                meshFilter.mesh = null;
                return;
            }

            colors.Clear();
            meshFilter.mesh = tileMesh;

            foreach (var area in AreaManager.Instance.Areas)
            {
                if (areaColorMap.ContainsKey(area.AreaID)) continue;
                
                var randColor = new Color(
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f),
                    overlayAlpha);
                areaColorMap.Add(area.AreaID, randColor);
            }

            areasText.text = "Areas: " + AreaManager.Instance.Areas.Count;
            
            tileMesh.GetColors(colors);
            var vertIndex = 0;

            for (var x = 0; x < World.Instance.Width; x++)
            for (var y = 0; y < World.Instance.Height; y++)
            {
                var tile = World.Instance.GetTileAt(x, y);
                var col = tile.Area != null ? areaColorMap[tile.Area.AreaID] : new Color(0, 0, 0, 0);
                colors[vertIndex] = col;
                colors[vertIndex + 1] = col;
                colors[vertIndex + 2] = col;
                colors[vertIndex + 3] = col;
                vertIndex += 4;
            }

            tileMesh.SetColors(colors);
        }

        public void Toggle()
        {
            enabled = !enabled;

            if (tileMesh == null) return;

            UpdateOverlay();
        }

        /// <summary>
        ///     Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateTileMesh()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
            var pivot = new Vector2(World.Instance.Width / 2.0f - 0.5f, World.Instance.Height / 2.0f - 0.5f);
            tileMesh = MeshUtils.CreateMesh("Area Debug Mesh", World.Instance.Width, World.Instance.Height, pivot);
        }
    }
}
using System.Collections.Generic;
using Models.Map;
using Models.Map.Areas;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers.Dev
{
    public class AreasDebugTool : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField, Range(0.0f, 1.0f)] private float overlayAlpha = 0.25f;

        [SerializeField] private TMP_Text areasText;

        private readonly Dictionary<int, Color> areaColorMap = new Dictionary<int, Color>();

        private Mesh tileMesh;

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

            var colors = meshFilter.mesh.colors;
            var vertIndex = 0;

            foreach (var tile in World.Instance)
            {
                var col = tile.Area != null ? areaColorMap[tile.Area.AreaID] : new Color(0, 0, 0, 0);
                colors[vertIndex] = col;
                colors[vertIndex + 1] = col;
                colors[vertIndex + 2] = col;
                colors[vertIndex + 3] = col;
                vertIndex += 4;
            }

            meshFilter.mesh.colors = colors;
        }

        public void Toggle()
        {
            enabled = !enabled;

            if (tileMesh == null) return;

            UpdateOverlay();
        }

        /// <summary>
        /// Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateTileMesh()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            meshFilter.mesh = null;

            tileMesh = new Mesh
            {
                name = "[Areas Debug Tool] Areas Debug Mesh",
                indexFormat = IndexFormat.UInt32
            };

            var combiner = new CombineInstance[World.Instance.Size];

            for (var x = 0; x < World.Instance.Width; x++)
            {
                for (var y = 0; y < World.Instance.Height; y++)
                {
                    var tileQuad = new Mesh();

                    var quadVerts = new Vector3[4];
                    quadVerts[0] = new Vector2(x - 0.5f, y - 0.5f);
                    quadVerts[1] = new Vector2(x - 0.5f, y + 0.5f);
                    quadVerts[2] = new Vector2(x + 0.5f, y + 0.5f);
                    quadVerts[3] = new Vector2(x + 0.5f, y - 0.5f);

                    var quadTris = new int[6];
                    quadTris[0] = 0;
                    quadTris[1] = 1;
                    quadTris[2] = 3;
                    quadTris[3] = 1;
                    quadTris[4] = 2;
                    quadTris[5] = 3;

                    var quadColors = new Color[quadVerts.Length];
                    for (var i = 0; i < quadColors.Length; i++)
                    {
                        quadColors[i] = new Color(0, 0, 0, 0);
                    }

                    tileQuad.vertices = quadVerts;
                    tileQuad.triangles = quadTris;
                    tileQuad.colors = quadColors;

                    combiner[x * World.Instance.Width + y].mesh = tileQuad;
                }
            }

            tileMesh.CombineMeshes(combiner, true, false);
            meshFilter.mesh = tileMesh;
        }
    }
}
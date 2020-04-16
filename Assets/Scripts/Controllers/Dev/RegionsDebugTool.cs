using Models.Map;
using Models.Map.Regions;
using Models.Map.Tiles;
using UnityEngine;
using UnityEngine.Rendering;

namespace Controllers.Dev
{
    public class RegionsDebugTool : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Color regionOverlayColor;

        private Mesh tileMesh;

        private void Start()
        {
            GenerateTileMesh();

            MouseController.Instance.mouseClickEvent += UpdateOverlay;
        }

        private void UpdateOverlay(Tile _tile, bool _ui)
        {
            if (!enabled || _tile == null)
            {
                meshFilter.mesh = null;
                return;
            }

            meshFilter.mesh = tileMesh;

            var tileRegion = RegionManager.Instance.GetID(_tile.Region);
            var colors = new Color[meshFilter.mesh.vertexCount];

            if (tileRegion != -1)
            {
                foreach (var tile in _tile.Region.Tiles)
                {
                    var vertIndex = (tile.X * World.Instance.Width + tile.Y) * 4;
                    colors[vertIndex] = regionOverlayColor;
                    colors[vertIndex + 1] = regionOverlayColor;
                    colors[vertIndex + 2] = regionOverlayColor;
                    colors[vertIndex + 3] = regionOverlayColor;
                }
            }

            meshFilter.mesh.colors = colors;
        }

        public void Toggle()
        {
            enabled = !enabled;
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
                name = "[Regions Debug Tool] Regions Debug Mesh",
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
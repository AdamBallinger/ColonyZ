using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Regions;
using ColonyZ.Models.Map.Tiles;
using ColonyZ.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace ColonyZ.Controllers.Dev
{
    public class RegionsDebugTool : MonoBehaviour
    {
        private Color clearColor = Color.clear;

        private bool displayLinkData;

        [SerializeField] private bool drawBridges;
        [SerializeField] private bool drawNeighbours;

        [SerializeField] private GameObject infoRoot;

        [SerializeField] private TMP_Text infoText;

        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Color regionBridgeColor;
        [SerializeField] private Color regionNeighbourColor;
        [SerializeField] private Color regionOverlayColor;

        private Tile selectedTile;

        private Color[] meshColors;
        private Mesh tileMesh;

        private void Start()
        {
            GenerateTileMesh();

            MouseController.Instance.mouseClickEvent += (btn, t, ui) =>
            {
                if (btn != MouseButton.LeftMouse) return;
                selectedTile = ui || MouseController.Instance.Mode == MouseMode.Process ? selectedTile : t;
                UpdateOverlay(selectedTile?.Region, ui);
            };

            MouseController.Instance.mouseTileChangeEvent += (t, ui) =>
            {
                if (t?.Region == null) return;
                if (t.Region == selectedTile?.Region) return;
                selectedTile = ui || MouseController.Instance.Mode == MouseMode.Process ? selectedTile : t;
                UpdateOverlay(selectedTile?.Region, ui);
            };

            RegionManager.Instance.regionsUpdateEvent += () => UpdateOverlay(selectedTile?.Region, false);
        }

        private void UpdateOverlay(Region _region, bool _ui)
        {
            if (!enabled || _region == null) return;

            if (_ui) return;

            for (var i = 0; i < meshColors.Length; i++)
            {
                meshColors[i] = clearColor;
            }

            infoText.text = string.Empty;

            if (displayLinkData)
            {
                foreach (var link in _region.Links)
                {
                    infoText.text += $"Hash: [{link.Span.UniqueHashCode()}]" +
                                     $"  Size: [{link.Span.Size}]" +
                                     $"  Dir: [{link.Span.Direction.ToString()}]\n";
                }
            }

            foreach (var tile in _region.Tiles)
            {
                var vertIndex = (tile.X * World.Instance.Width + tile.Y) * 4;
                meshColors[vertIndex] = regionOverlayColor;
                meshColors[vertIndex + 1] = regionOverlayColor;
                meshColors[vertIndex + 2] = regionOverlayColor;
                meshColors[vertIndex + 3] = regionOverlayColor;
            }

            if (drawNeighbours)
            {
                foreach (var link in _region.Links)
                {
                    var neighbour = link.GetOther(_region);
                    if (neighbour == null) break;

                    foreach (var tile in neighbour.Tiles)
                    {
                        var vertIndex = (tile.X * World.Instance.Width + tile.Y) * 4;
                        meshColors[vertIndex] = regionNeighbourColor;
                        meshColors[vertIndex + 1] = regionNeighbourColor;
                        meshColors[vertIndex + 2] = regionNeighbourColor;
                        meshColors[vertIndex + 3] = regionNeighbourColor;
                    }
                }
            }

            if (drawBridges)
            {
                foreach (var pair in _region.AccessMap)
                {
                    foreach (var tile in pair.Value)
                    {
                        var vertIndex = (tile.X * World.Instance.Width + tile.Y) * 4;
                        meshColors[vertIndex] = regionBridgeColor;
                        meshColors[vertIndex + 1] = regionBridgeColor;
                        meshColors[vertIndex + 2] = regionBridgeColor;
                        meshColors[vertIndex + 3] = regionBridgeColor;
                    }
                }
            }

            tileMesh.SetColors(meshColors);
        }

        public void Toggle()
        {
            enabled = !enabled;
            meshFilter.mesh = enabled ? tileMesh : null;
            infoRoot.SetActive(enabled);
        }

        /// <summary>
        ///     Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateTileMesh()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

            meshFilter.mesh = null;
            var pivot = new Vector2(World.Instance.Width / 2.0f - 0.5f, World.Instance.Height / 2.0f - 0.5f);
            tileMesh = MeshUtils.CreateMesh("Regions Debug Mesh", World.Instance.Width, World.Instance.Height, pivot);
            meshFilter.mesh = tileMesh;
            meshColors = new Color[tileMesh.vertexCount];
        }

        public void ToggleNeighbours()
        {
            drawNeighbours = !drawNeighbours;
        }

        public void ToggleBridges()
        {
            drawBridges = !drawBridges;
        }

        public void ToggleLinks()
        {
            displayLinkData = !displayLinkData;
        }
    }
}
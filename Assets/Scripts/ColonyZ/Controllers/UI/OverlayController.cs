using ColonyZ.Models.Map;
using ColonyZ.Models.UI;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.UI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class OverlayController : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        /// <summary>
        /// Texture used by shader to draw the overlay icons.
        /// </summary>
        private Texture2D overlayTexture;

        private Color[] colors;

        private int width, height;

        private void Start()
        {
            width = World.Instance.Width;
            height = World.Instance.Height;
            var pivot = new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f);
            meshFilter.mesh = MeshUtils.CreateQuad("Overlay Mesh", width, height, pivot);
            
            GenerateOverlayTexture();
            
            meshRenderer.material.mainTexture = overlayTexture;
            meshRenderer.material.SetInt("_WorldWidth", width);
            meshRenderer.material.SetInt("_WorldHeight", height);
            
            World.Instance.Overlay.overlaySingleUpdatedEvent += UpdateOverlaySingle;
            World.Instance.Overlay.overlayUpdatedEvent += UpdateOverlay;
            
            ClearOverlay();
        }

        private void GenerateOverlayTexture()
        {
            if (overlayTexture != null) return;
            
            overlayTexture = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };

            colors = new Color[width * height];
        }
        
        private void ClearOverlay()
        {
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                World.Instance.Overlay.SetOverlayAtTile(World.Instance.GetTileAt(x * width + y), OverlayType.None);
            }
            
            overlayTexture.SetPixels(colors);
            overlayTexture.Apply();
        }

        public void UpdateOverlaySingle(Vector2Int _pos)
        {
            var colorIndex = _pos.x + width * _pos.y;
            var overlayIndex = _pos.x * width + _pos.y;
            var overlayType = World.Instance.Overlay.OverlayArray[overlayIndex] / 255.0f;
            colors[colorIndex] = new Color(overlayType, 0, 0, World.Instance.Overlay.OverlayAlpha[overlayIndex]);
            
            overlayTexture.SetPixels(colors);
            overlayTexture.Apply();
        }
        
        private void UpdateOverlay()
        {
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var index = World.Instance.Overlay.OverlayArray[x * width + y] / 255.0f;
                colors[x + width * y] = new Color(index, 0, 0, World.Instance.Overlay.OverlayAlpha[x * width + y]);
            }
            
            overlayTexture.SetPixels(colors);
            overlayTexture.Apply();
        }
    }
}
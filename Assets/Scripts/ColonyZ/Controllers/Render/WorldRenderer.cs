using System.Collections.Generic;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Rendering;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    public class WorldRenderer
    {
        private List<WorldSection> sections;

        public WorldRenderer()
        {
            sections = new List<WorldSection>(World.Instance.WorldGrid.Chunks.Count);
            Debug.Log($"Created world renderer with {sections.Capacity} section capacity.");
        }

        public void RenderSections()
        {
            // TODO: Go through each section and render each layer in correct order with Graphics.DrawMesh
            foreach (var section in sections)
            {
                
            }
        }




        // public void GenerateWorldMesh(int _width, int _height)
        // {
        //     Width = _width;
        //     Height = _height;
        //     
        //     // if (meshRenderer == null)
        //     // {
        //     //     meshRenderer = GetComponent<MeshRenderer>();
        //     //     meshFilter = GetComponent<MeshFilter>();
        //     // }
        //
        //     // offset centre by -0.5f since sprites pivot from centre.
        //     var meshPivot = new Vector2(Width / 2.0f - 0.5f, Height / 2.0f - 0.5f);
        //
        //     // meshFilter.mesh = MeshUtils.CreateQuad("World Quad", Width, Height, meshPivot);
        //     // meshRenderer.material.mainTexture = GenerateMapTexture();
        //     // meshRenderer.material.SetInt("WorldWidth", Width);
        //     // meshRenderer.material.SetInt("WorldHeight", Height);
        // }

        // private Texture2D GenerateMapTexture()
        // {
        //     mapTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false)
        //     {
        //         filterMode = FilterMode.Point
        //     };
        //
        //     colors = new Color[mapTexture.width * mapTexture.height];
        //     for (var x = 0; x < Width; x++)
        //     for (var y = 0; y < Height; y++)
        //     {
        //         var index = x * Width + y;
        //         var tileIndex = World.Instance.GetTileAt(x, y).TileDefinition.TextureIndex / 255.0f;
        //         var maskIndex = 0 / 255.0f;
        //         colors[index] = new Color(tileIndex, maskIndex, 0);
        //     }
        //
        //     mapTexture.SetPixels(colors);
        //     mapTexture.Apply();
        //
        //     return mapTexture;
        // }
    }
}
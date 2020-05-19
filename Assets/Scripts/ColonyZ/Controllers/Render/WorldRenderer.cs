using ColonyZ.Models.Map;
using ColonyZ.Models.Sprites;
using UnityEngine;
using UnityEngine.Rendering;

namespace ColonyZ.Controllers.Render
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class WorldRenderer : MonoBehaviour
    {
        private MeshFilter meshFilter;

        private MeshRenderer meshRenderer;

        [SerializeField] private Texture2D tileTypesTexture;

        private void Start()
        {
            SliceTileTypesTexture();
        }

        /// <summary>
        ///     Generates the mesh for the world.
        /// </summary>
        public void GenerateWorldMesh(int _width, int _height)
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
                meshFilter = GetComponent<MeshFilter>();
            }

            var mesh = new Mesh
            {
                name = "World Mesh",
                indexFormat = IndexFormat.UInt16
            };

            var verts = new Vector3[4];
            verts[0] = new Vector3(-0.5f, -0.5f);
            verts[1] = new Vector3(-0.5f, _height - 0.5f);
            verts[2] = new Vector3(_width - 0.5f, _height - 0.5f);
            verts[3] = new Vector3(_width - 0.5f, -0.5f);

            var tris = new int[6];
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 3;
            tris[3] = 1;
            tris[4] = 2;
            tris[5] = 3;

            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uv;

            meshFilter.mesh = mesh;
            meshRenderer.material.mainTexture = GenerateMapTexture(_width, _height);
            meshRenderer.material.SetInt("_WorldWidth", _width);
            meshRenderer.material.SetInt("_WorldHeight", _height);
        }

        private Texture2D GenerateMapTexture(int _width, int _height)
        {
            var texture = new Texture2D(_width, _height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Point
            };

            var colors = new Color[texture.width * texture.height];
            for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
            {
                var index = x * _width + y;
                var tileIndex = World.Instance.GetTileAt(x, y).TileDefinition.TextureIndex;
                colors[index] = new Color(tileIndex, tileIndex, tileIndex);
            }

            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }

        /// <summary>
        ///     Cuts the tile types texture up into individual sprites.
        /// </summary>
        private void SliceTileTypesTexture()
        {
            for (var y = 0; y < tileTypesTexture.height / 32; y++)
            for (var x = 0; x < tileTypesTexture.width / 32; x++)
            {
                var sprite = Sprite.Create(tileTypesTexture, new Rect(x, y, 32, 32), new Vector2(x + 32, y + 32));
                SpriteCache.AddSprite("Tiles", sprite);
            }
        }
    }
}
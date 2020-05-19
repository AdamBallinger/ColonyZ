using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using UnityEngine;
using UnityEngine.Rendering;

namespace ColonyZ.Controllers.Dev
{
    public class TileNodesTool : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Texture2D nodesTexture;

        private void Start()
        {
            NodeGraph.Instance.RegisterGraphUpdateCallback(OnNodesUpdated);
        }

        private void OnEnable()
        {
            GenerateTileMesh();
        }

        private void OnDisable()
        {
            meshFilter.mesh = null;
        }

        private void OnNodesUpdated()
        {
            if (!enabled) return;

            GenerateTileMesh();
        }

        /// <summary>
        ///     Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateTileMesh()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

            meshFilter.mesh = null;

            var mesh = new Mesh
            {
                name = "[Tile Nodes Tool] Node Mesh",
                indexFormat = IndexFormat.UInt32
            };

            var combiner = new CombineInstance[World.Instance.Size];

            for (var x = 0; x < NodeGraph.Instance.Width; x++)
            for (var y = 0; y < NodeGraph.Instance.Height; y++)
            {
                var tile = World.Instance.GetTileAt(x, y);

                var nodesMesh = new Mesh();

                var quadVerts = new Vector3[4];
                quadVerts[0] = new Vector2(x - 0.5f, y - 0.5f);
                quadVerts[1] = new Vector2(x - 0.5f, y + 0.5f);
                quadVerts[2] = new Vector2(x + 0.5f, y + 0.5f);
                quadVerts[3] = new Vector2(x + 0.5f, y - 0.5f);

                var quadUV = new Vector2[4];

                // Calculate the number of textures inside the texture sheet
                var texturesX = nodesTexture.width / 32;
                var texturesY = nodesTexture.height / 32;

                var uSize = 1.0f / texturesX;
                var vSize = 1.0f / texturesY;

                var nodeTextureIndex = (int) tile.GetEnterability();

                // Calculate nodes X and Y inside the texture
                var nodeX = nodeTextureIndex % texturesX;
                var nodeY = (nodeTextureIndex - nodeX) / texturesX;

                // Generate UV for the bottom left vertex of the quad
                var u = uSize * nodeX;
                // invert the v as it starts at lower left rather than top left. Minus 1 so v points to bottom vertex
                var v = vSize * (texturesY - nodeY - 1);

                // Set the UV for the tile quad
                quadUV[0] = new Vector2(u, v);
                quadUV[1] = new Vector2(u, v + vSize);
                quadUV[2] = new Vector2(u + uSize, v + vSize);
                quadUV[3] = new Vector2(u + uSize, v);

                var quadTris = new int[6];
                quadTris[0] = 0;
                quadTris[1] = 1;
                quadTris[2] = 3;
                quadTris[3] = 1;
                quadTris[4] = 2;
                quadTris[5] = 3;

                nodesMesh.vertices = quadVerts;
                nodesMesh.triangles = quadTris;
                nodesMesh.uv = quadUV;

                combiner[x * World.Instance.Width + y].mesh = nodesMesh;
            }

            mesh.CombineMeshes(combiner, true, false);
            meshFilter.mesh = mesh;
        }
    }
}
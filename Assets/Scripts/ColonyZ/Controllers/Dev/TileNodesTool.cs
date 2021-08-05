using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Pathing;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.Dev
{
    public class TileNodesTool : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Texture2D nodesTexture;

        private Mesh tileMesh;

        private void Start()
        {
            GenerateTileMesh();
            
            NodeGraph.Instance.RegisterGraphUpdateCallback(OnNodesUpdated);
        }

        public void Toggle()
        {
            enabled = !enabled;

            if (!enabled)
                meshFilter.mesh = null;
            else
                UpdateUVs();
        }

        private void OnNodesUpdated()
        {
            UpdateUVs();
        }

        private void UpdateUVs()
        {
            if (!enabled || tileMesh == null) return;
            
            var uvs = new Vector2[tileMesh.vertexCount];

            for (var i = 0; i < uvs.Length; i += 4)
            {
                var tile = World.Instance.GetTileAt(i / 4);
                var node = NodeGraph.Instance.GetNodeAt(i / 4);

                // Calculate the number of textures inside the texture sheet
                var texturesX = nodesTexture.width / 32;
                var texturesY = nodesTexture.height / 32;

                var uSize = 1.0f / texturesX;
                var vSize = 1.0f / texturesY;

                var nodeTextureIndex = node.Pathable ? (int) tile.GetEnterability() : 1;

                // Calculate nodes X and Y inside the texture
                var nodeX = nodeTextureIndex % texturesX;
                var nodeY = (nodeTextureIndex - nodeX) / texturesX;

                // Generate UV for the bottom left vertex of the quad
                var u = uSize * nodeX;
                // invert the v as it starts at lower left rather than top left. Minus 1 so v points to bottom vertex
                var v = vSize * (texturesY - nodeY - 1);

                uvs[i] = new Vector2(u, v);
                uvs[i + 1] = new Vector2(u, v + vSize);
                uvs[i + 2] = new Vector2(u + uSize, v + vSize);
                uvs[i + 3] = new Vector2(u + uSize, v);
            }

            tileMesh.SetUVs(0, uvs);

            meshFilter.mesh = tileMesh;
        }

        /// <summary>
        ///     Generates the full tilemap mesh for the world
        /// </summary>
        private void GenerateTileMesh()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

            var pivot = new Vector2(NodeGraph.Instance.Width / 2.0f - 0.5f, NodeGraph.Instance.Height / 2.0f - 0.5f);
            tileMesh = MeshUtils.CreateMesh("Tile Nodes Mesh", NodeGraph.Instance.Width, NodeGraph.Instance.Height, pivot);
            UpdateUVs();
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;

namespace ColonyZ.Utils
{
    public static class MeshUtils
    {
        /// <summary>
        ///     Creates a quad with the given width and height with the central pivot at given centre position.
        /// </summary>
        /// <param name="_meshName"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_pivot"></param>
        /// <returns></returns>
        public static Mesh CreateQuad(string _meshName, int _width, int _height, Vector2 _pivot)
        {
            var mesh = CreateQuad(_width, _height, _pivot);
            mesh.name = _meshName;
            return mesh;
        }

        /// <summary>
        ///     Creates a mesh made up of width * height quads. Each quad has its own 4 vertices.
        ///     Vertex colors are default to 0,0,0,0.
        /// </summary>
        /// <param name="_meshName"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_pivot"></param>
        /// <returns></returns>
        public static Mesh CreateMesh(string _meshName, int _width, int _height, Vector2 _pivot)
        {
            var mesh = new Mesh();
            mesh.name = _meshName;
            mesh.indexFormat = IndexFormat.UInt32;

            var combiner = new CombineInstance[_width * _height];
            
            for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
            {
                var offset = new Vector2(_width / 2 - x - 0.5f, _height / 2 - y - 0.5f);
                var quad = CreateQuad(1, 1, _pivot - offset);

                quad.colors = new Color[4];
                combiner[x * _width + y].mesh = quad;
            }
            
            mesh.CombineMeshes(combiner, true, false);

            return mesh;
        }

        public static Mesh CreateQuad(int _w, int _h, Vector2 _pivot)
        {
            var mesh = new Mesh
            {
                name = "Quad",
                indexFormat = IndexFormat.UInt16
            };
            
            var hw = _w / 2.0f;
            var hh = _h / 2.0f;
            
            var verts = new Vector3[4];
            verts[0] = new Vector3(_pivot.x - hw, _pivot.y - hh);
            verts[1] = new Vector3(_pivot.x - hw, _pivot.y + hh);
            verts[2] = new Vector3(_pivot.x + hw, _pivot.y + hh);
            verts[3] = new Vector3(_pivot.x + hw, _pivot.y - hh);
            
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

            return mesh;
        }
    }
}
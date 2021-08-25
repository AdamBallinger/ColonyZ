using System.Collections.Generic;
using ColonyZ.Models.Map;
using ColonyZ.Models.TimeSystem;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    public class FoliageRenderer : MonoBehaviour
    {
        [SerializeField] private Material foliageMaterial;

        private readonly int materialSpeedMultiplierID = Shader.PropertyToID("speedMultiplier");

        private Dictionary<Vector2, Mesh> meshes = new Dictionary<Vector2, Mesh>(2);

        private void Start()
        {
            TimeManager.Instance.timeModeChangedEvent += TimeModeChanged;
            TimeModeChanged(TimeManager.Instance.TimeMode);
        }

        private void TimeModeChanged(TimeMode _mode)
        {
            foliageMaterial.SetFloat(materialSpeedMultiplierID, (int)_mode);
        }

        private void Update()
        {
            foreach (var foliage in World.Instance.Foliage)
            {
                if (!foliage.ObjectData.Foliage)
                {
                    Debug.LogError("Attempted to render a none foliage object with FoliageRenderer. Skipping object.");
                    continue;
                }
                
                var size = foliage.ObjectData.SpriteData.Sprites[0].rect.size / 32;

                if (!meshes.TryGetValue(size, out var mesh))
                {
                    var pivot = foliage.ObjectData.SpriteData.Sprites[0].bounds.center;
                    meshes.Add(size, MeshUtils.CreateQuad("Foliage_Quad", (int)size.x, (int)size.y, pivot));
                    mesh = meshes[size];
                }

                Graphics.DrawMesh(mesh, foliage.OriginTile.Position, Quaternion.identity, foliageMaterial, 0);
            }
        }
    }
}
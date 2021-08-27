using System.Collections.Generic;
using ColonyZ.Models.Map;
using ColonyZ.Models.Map.Tiles.Objects;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Sprites;
using ColonyZ.Models.TimeSystem;
using ColonyZ.Utils;
using UnityEngine;

namespace ColonyZ.Controllers.Render
{
    public class ObjectsRenderer : MonoBehaviour
    {
        [SerializeField] private Material basicMaterial;
        [SerializeField] private Material foliageMaterial;

        private readonly int materialSpeedMultiplierID = Shader.PropertyToID("speedMultiplier");

        private Dictionary<TileObjectData, Mesh> meshes = new Dictionary<TileObjectData, Mesh>();

        private Camera cam;

        private Dictionary<TileObjectData, MaterialPropertyBlock> mpbs = new Dictionary<TileObjectData, MaterialPropertyBlock>();

        private void Start()
        {
            cam = Camera.main;
            TimeManager.Instance.timeModeChangedEvent += TimeModeChanged;
            TimeModeChanged(TimeManager.Instance.TimeMode);

            foreach (var data in TileObjectDataCache.ObjectDatas)
            {
                if (data.SmartObject) continue;
                
                var pivot = data.SpriteData.Sprites[0].bounds.center;
                var size = data.SpriteData.Sprites[0].rect.size / 32;
                //Debug.Log(data.ObjectName + " : " + data.SpriteData.Sprites[0].rect);
                meshes.Add(data, MeshUtils.CreateQuad("Quad", (int)size.x, (int)size.y, pivot));
                var mpb = new MaterialPropertyBlock();
                mpb.SetTexture("_MainTex", SpriteCache.GetSprite(data.SpriteData.SpriteGroup, 0).texture);
                mpbs.Add(data, mpb);
            }
        }

        private void TimeModeChanged(TimeMode _mode)
        {
            foliageMaterial.SetFloat(materialSpeedMultiplierID, (int)_mode);
        }

        private void Update()
        {
            foreach (var obj in World.Instance.Objects)
            {
                // Dont draw smart objects (Walls, stone, etc.) as they are drawn with the tile shader.
                if (obj.ObjectData.SmartObject) continue;
                
                // Check if the object is in view of the camera (Plus a small border to prevent pop in).
                var screenPos = cam.WorldToViewportPoint(obj.OriginTile.Position); 
                var inView = screenPos.z >= -0.1f && 
                             screenPos.x >= -0.1f && 
                             screenPos.x <= 1.1f && 
                             screenPos.y >= -0.1f && 
                             screenPos.y <= 1.1f;
                
                if (!inView) continue;

                Graphics.DrawMesh(meshes[obj.ObjectData],
                    obj.OriginTile.Position,
                    ObjectRotationUtil.GetQuaternion(obj.ObjectRotation),
                    obj.ObjectData.Foliage ? foliageMaterial : basicMaterial,
                    1,
                    cam,
                    0,
                    mpbs[obj.ObjectData]);
            }
        }
    }
}
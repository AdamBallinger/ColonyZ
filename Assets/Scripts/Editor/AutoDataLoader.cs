using ColonyZ.Controllers.Loaders;
using ColonyZ.Models.Items;
using ColonyZ.Models.Map.Tiles.Objects.Data;
using ColonyZ.Models.Sprites;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(DataLoader))]
    public class AutoDataLoader : UnityEditor.Editor
    {
        private DataLoader Target => (DataLoader) serializedObject.targetObject;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LoadData<SpriteData>("spriteData");
            LoadData<TileObjectData>("objectData");
            LoadData<Item>("itemData");
        }

        private void LoadData<T>(string _arrayName) where T : ScriptableObject
        {
            serializedObject.Update();

            var arrayProperty = serializedObject.FindProperty(_arrayName);
            arrayProperty.ClearArray();

            var guids = AssetDatabase.FindAssets("t:" + typeof(T));
            arrayProperty.arraySize = guids.Length;

            for (var i = 0; i < guids.Length; i++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue = asset;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace EzPool
{
    [CustomEditor(typeof(EzPoolManager))]
    public class EzPoolManagerEditor : Editor
    {
        private EzPoolManager Target => (EzPoolManager)target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var pooledObj = serializedObject.FindProperty("pooledPrefab");
            var maxPoolCount = serializedObject.FindProperty("maxPoolCount");
            var prePoolCount = serializedObject.FindProperty("prePoolCount");

            EditorGUILayout.LabelField("Prefab Settings", EditorStyles.helpBox);
            pooledObj.objectReferenceValue = EditorGUILayout.ObjectField("Pooled Prefab", pooledObj.objectReferenceValue, typeof(GameObject), false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Pooling Settings", EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxPoolCount"), true);
            EditorGUILayout.IntSlider(serializedObject.FindProperty("prePoolCount"), 0, maxPoolCount.intValue);

            if (maxPoolCount.intValue < 1)
            {
                serializedObject.FindProperty("maxPoolCount").intValue = 1;
            }

            if(prePoolCount.intValue > maxPoolCount.intValue)
            {
                serializedObject.FindProperty("prePoolCount").intValue = maxPoolCount.intValue;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pool Manager Stats", EditorStyles.helpBox);

            var activeObjCount = (float)serializedObject.FindProperty("activeObjects").arraySize;
            var inactiveObjCount = (float) serializedObject.FindProperty("inactiveObjects").arraySize;

            ProgressBar((float)Target.GetPoolCount() / maxPoolCount.intValue, $"Pooled Object Count : {Target.GetPoolCount()}");
            ProgressBar(activeObjCount / maxPoolCount.intValue, $"Active Pool Objects : {activeObjCount}/{maxPoolCount.intValue}");
            ProgressBar(inactiveObjCount / maxPoolCount.intValue, $"Inactive Pool Objects : {inactiveObjCount}/{maxPoolCount.intValue}");

            serializedObject.ApplyModifiedProperties();
        }

        private void ProgressBar(float _value, string _label)
        {
            var rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, _value, _label);
            EditorGUILayout.Space();
        }
    }
}
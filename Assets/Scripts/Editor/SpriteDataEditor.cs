using ColonyZ.Models.Sprites;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(SpriteData))]
    public class SpriteDataEditor : UnityEditor.Editor
    {
        private SpriteData Target => (SpriteData) serializedObject.targetObject;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            if (Target.Sprites != null && Target.SpriteCount > 1)
                EditorGUILayout.LabelField("Ui Icon Index: " + serializedObject.FindProperty("uiIconIndex").intValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
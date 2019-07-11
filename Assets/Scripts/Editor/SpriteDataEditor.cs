using Models.Sprites;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(SpriteData))]
    public class SpriteDataEditor : UnityEditor.Editor
    {
        private SpriteData Target => (SpriteData) target;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();
            
            if (Target.SpriteCount > 1)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("uiIconIndex"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

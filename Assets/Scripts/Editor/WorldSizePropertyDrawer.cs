using ColonyZ.Models.Map;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(WorldSizeTypes.WorldSize))]
    public class WorldSizePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var sizes = new string[WorldSizeTypes.SIZES.Count];
            var index = 0;

            // Get the auto generated field names for auto properties.
            var w = property.FindPropertyRelative("<Width>k__BackingField");
            var h = property.FindPropertyRelative("<Height>k__BackingField");

            for (var i = 0; i < sizes.Length; i++)
            {
                var size = WorldSizeTypes.SIZES[i];
                if (size.Width == w.intValue && size.Height == h.intValue)
                    index = i;
                sizes[i] = size.ToString();
            }

            //EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(position, label.text, index, sizes);

            if (EditorGUI.EndChangeCheck())
            {
                var size = WorldSizeTypes.SIZES[index];
                w.intValue = size.Width;
                h.intValue = size.Height;
            }

            //EditorGUI.EndProperty();
        }
    }
}
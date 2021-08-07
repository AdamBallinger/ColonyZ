using System;
using System.Collections.Generic;
using ColonyZ.Models.Sprites;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SpriteDataTool : EditorWindow
    {
        private enum ToolMode
        {
            Menu,
            Create,
            Edit
        }
        
        private const string DATA_PATH_ROOT = "Assets/Data/Sprites/";

        private ToolMode mode = ToolMode.Menu;

        private SpriteData currentAsset;

        private Texture2D texture;

        private List<SpriteMetaData> spriteMetaDatas;

        private int cellWidth, cellHeight;

        private string assetName;
        
        private Color defaultGUIColor;
        
        [MenuItem("ColonyZ/Sprite Data Tool")]
        private static void Init()
        {
            var window = GetWindow<SpriteDataTool>(true, "Sprite Data Tool");
            window.defaultGUIColor = GUI.color;
            window.Show();
        }

        private void OnGUI()
        {
            GUI.color = defaultGUIColor;
            EditorStyles.helpBox.fontSize = 14;
            EditorStyles.helpBox.fontStyle = FontStyle.Bold;

            switch (mode)
            {
                case ToolMode.Menu:
                    MenuModeGUI();
                    return;
                case ToolMode.Create:
                    CreateModeGUI();
                    return;
                case ToolMode.Edit:
                    EditModeGUI();
                    return;
            }
        }

        private void MenuModeGUI()
        {
            EditorGUILayout.LabelField("Sprite Data Tool", GUIStyle.none);
            if (GUILayout.Button("Create"))
            {
                mode = ToolMode.Create;
                spriteMetaDatas = new List<SpriteMetaData>();
                currentAsset = CreateSpriteData();
                assetName = string.Empty;
                cellWidth = 32;
                cellHeight = 32;
            }
            if (GUILayout.Button("Edit"))
            {
                mode = ToolMode.Edit;
            }
        }

        private void CreateModeGUI()
        {
            EditorGUILayout.LabelField("Create new sprite data asset.", GUIStyle.none);
            if (GUILayout.Button("Cancel"))
            {
                GUI.FocusControl(null);
                mode = ToolMode.Menu;
                texture = null;
            }

            assetName = EditorGUILayout.TextField("Sprite Data Name: ", assetName);
            EditorGUILayout.LabelField("Asset save directory: " + DATA_PATH_ROOT + assetName + ".asset");
            
            EditorGUILayout.Space(4);
            texture = (Texture2D)EditorGUILayout.ObjectField("Sprite Texture", texture, typeof(Texture2D), false);
            EditorGUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            {
                cellWidth = EditorGUILayout.IntField("Cell Width:", cellWidth);
                cellHeight = EditorGUILayout.IntField("Cell Height:", cellHeight);
            }
            EditorGUILayout.EndHorizontal();

            if (texture == null)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.HelpBox("No texture selected.", MessageType.Warning);
                return;
            }
            
            if (GUILayout.Button("Slice Texture"))
            {
                spriteMetaDatas.Clear();
                // TODO: Slice texture into separate sprites.
                for (var x = 0; x < texture.width; x += cellWidth)
                for (var y = 0; y < texture.height; y+= cellHeight)
                {
                    var metaData = new SpriteMetaData();
                    metaData.name = texture.name + "_" + spriteMetaDatas.Count;
                    metaData.rect = new Rect(x, y, cellWidth, cellHeight);
                    metaData.alignment = 0;
                    metaData.pivot = Vector2.zero;
                    spriteMetaDatas.Add(metaData);
                }

                Slice();
            }

            EditorGUILayout.LabelField("Sprite count: " + spriteMetaDatas.Count);
            
            EditorGUILayout.Space(6);

            if (spriteMetaDatas.Count > 0)
            {
                if (GUILayout.Button("Save Asset"))
                {
                    SaveAsset(currentAsset, DATA_PATH_ROOT, assetName);
                }
            }
            else if (assetName == String.Empty)
            {
                GUI.color = Color.red;
                EditorGUILayout.HelpBox("No asset name specified.", MessageType.Error);
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.HelpBox("No sprites to create asset.", MessageType.Error);
            }
        }

        private void EditModeGUI()
        {
            EditorGUILayout.LabelField("Edit sprite data asset.", GUIStyle.none);
            if (GUILayout.Button("Cancel"))
            {
                GUI.FocusControl(null);
                mode = ToolMode.Menu;
            }
        }

        private SpriteData CreateSpriteData()
        {
            return CreateInstance<SpriteData>();
        }

        private void SaveAsset(SpriteData _data, string _path, string _assetName)
        {
            AssetDatabase.CreateAsset(_data, _path + _assetName + ".asset");
            AssetDatabase.SaveAssets();
        }

        private void DeleteAsset(SpriteData _data)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_data));
        }

        private void Slice()
        {
            var textureImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;

            if (textureImporter == null)
            {
                Debug.LogError("Texture importer is null.");
                return;
            }
            
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;
            textureImporter.spritePixelsPerUnit = 32;

            textureImporter.spritesheet = spriteMetaDatas.ToArray();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(texture), ImportAssetOptions.ForceUpdate);
        }
    }
}
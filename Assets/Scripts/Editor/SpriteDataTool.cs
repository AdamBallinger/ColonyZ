using System;
using System.Collections.Generic;
using System.Linq;
using ColonyZ.Models.Sprites;
using UnityEditor;
using UnityEditorInternal;
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

        private SpriteData currentAsset, previousAsset;

        private Texture2D texture;

        private List<SpriteMetaData> spriteMetaDatas = new List<SpriteMetaData>();

        private int cellWidth, cellHeight;

        private Vector2 spritePivot;

        private int uiIconIndex;

        private string assetName;
        
        private Color defaultGUIColor;

        private string texturePath => AssetDatabase.GetAssetPath(texture);
        private string assetPath => AssetDatabase.GetAssetPath(currentAsset);
        
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
                spriteMetaDatas.Clear();
                currentAsset = CreateSpriteData();
                assetName = string.Empty;
                texture = null;
                cellWidth = 32;
                cellHeight = 32;
                spritePivot = new Vector2(16, 16);
            }
            if (GUILayout.Button("Edit"))
            {
                mode = ToolMode.Edit;
                currentAsset = null;
                texture = null;
                previousAsset = null;
            }
        }

        private void CreateModeGUI()
        {
            EditorGUILayout.LabelField("Create new sprite data asset.", GUIStyle.none);
            if (GUILayout.Button("Cancel"))
            {
                GUI.FocusControl(null);
                mode = ToolMode.Menu;
            }

            assetName = EditorGUILayout.TextField("Sprite Data Name: ", assetName);
            EditorGUILayout.LabelField("Asset save directory: " + DATA_PATH_ROOT + assetName + ".asset");
            
            EditorGUILayout.Space(4);
            texture = (Texture2D)EditorGUILayout.ObjectField("Sprite Texture", texture, typeof(Texture2D), false);
            EditorGUILayout.Space(4);
            
            if (texture == null)
            {
                EditorGUILayout.HelpBox("No texture selected.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                cellWidth = EditorGUILayout.IntField("Cell Width:", cellWidth);
                cellHeight = EditorGUILayout.IntField("Cell Height:", cellHeight);
            }
            EditorGUILayout.EndHorizontal();

            spritePivot = EditorGUILayout.Vector2Field("Sprite pivot: ", spritePivot);
            spritePivot.x = Mathf.Min(spritePivot.x, cellWidth);
            spritePivot.y = Mathf.Min(spritePivot.y, cellHeight);

            if (GUILayout.Button("Slice Texture"))
            {
                Slice();
            }
            
            EditorGUILayout.LabelField("Sprite count: " + spriteMetaDatas.Count);

            if (spriteMetaDatas.Count > 0)
            {
                uiIconIndex = EditorGUILayout.IntSlider("UI Icon Index: ", uiIconIndex, 0, spriteMetaDatas.Count - 1);
                DrawUIIconSprite(uiIconIndex);
            }

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

            currentAsset = (SpriteData)EditorGUILayout.ObjectField("Asset: ", currentAsset, typeof(SpriteData), false);

            if (currentAsset == null)
            {
                EditorGUILayout.HelpBox("Select a sprite data asset to edit.", MessageType.Info);
                return;
            }
            
            var serializedObject = new SerializedObject(currentAsset);
            var spriteGroup = serializedObject.FindProperty("spriteGroupName");
            var spriteUIIconIndex = serializedObject.FindProperty("uiIconIndex");
            var sprites = serializedObject.FindProperty("sprites");

            if (currentAsset.name != previousAsset?.name)
            {
                assetName = currentAsset.name;
                var sprite = (Sprite)sprites.GetArrayElementAtIndex(0).objectReferenceValue;
                texture = sprite.texture;
                cellWidth = (int)sprite.rect.width;
                cellHeight = (int)sprite.rect.height;
                spritePivot = sprite.pivot;
            }

            EditorGUILayout.LabelField("Src: " + AssetDatabase.GetAssetPath(currentAsset));
            
            assetName = EditorGUILayout.TextField("Asset name: ", assetName);

            spriteGroup.stringValue = EditorGUILayout.TextField("Sprite group: ", spriteGroup.stringValue);

            texture = (Texture2D)EditorGUILayout.ObjectField("Texture: ", texture, typeof(Texture2D), false);

            if (texture != null)
            {
                spriteUIIconIndex.intValue = EditorGUILayout.IntSlider("UI Icon Index: ", 
                    spriteUIIconIndex.intValue, 0, sprites.arraySize - 1);
                DrawUIIconSprite(spriteUIIconIndex.intValue);
            }

            EditorGUILayout.Space(4);
            
            EditorGUILayout.BeginHorizontal();
            {
                cellWidth = EditorGUILayout.IntField("Cell Width:", cellWidth);
                cellHeight = EditorGUILayout.IntField("Cell Height:", cellHeight);
            }
            EditorGUILayout.EndHorizontal();
            
            spritePivot = EditorGUILayout.Vector2Field("Sprite pivot: ", spritePivot);
            spritePivot.x = Mathf.Min(spritePivot.x, cellWidth);
            spritePivot.y = Mathf.Min(spritePivot.y, cellHeight);

            if (GUILayout.Button("Re-Slice"))
            {
                Slice();
                SaveSpritesToAsset(serializedObject);
            }
            
            EditorGUILayout.LabelField("Sprites: " + serializedObject.FindProperty("sprites").arraySize);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Save"))
                {
                    AssetDatabase.RenameAsset(assetPath, assetName);
                    Slice();
                    SaveSpritesToAsset(serializedObject);
                    AssetDatabase.SaveAssets();
                }

                GUI.color = Color.red;
                EditorGUILayout.Space(4);
                if (GUILayout.Button("Delete"))
                {
                    DeleteAsset(currentAsset);
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
            previousAsset = currentAsset;
        }

        private void DrawUIIconSprite(int _index)
        {
            GUILayout.Label(AssetPreview.GetAssetPreview(AssetDatabase.LoadAllAssetsAtPath(texturePath)
                .OfType<Sprite>().ToArray()[_index]));
        }

        private SpriteData CreateSpriteData()
        {
            return CreateInstance<SpriteData>();
        }

        private void SaveAsset(SpriteData _data, string _path, string _assetName)
        {
            var serializedObject = new SerializedObject(_data);
            SaveSpritesToAsset(serializedObject);

            serializedObject.FindProperty("spriteGroupName").stringValue = assetName;
            serializedObject.FindProperty("uiIconIndex").intValue = uiIconIndex;
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.CreateAsset(_data, _path + _assetName + ".asset");
            AssetDatabase.SaveAssets();
        }

        private void DeleteAsset(SpriteData _data)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_data));
            currentAsset = null;
        }

        private void Slice()
        {
            spriteMetaDatas.Clear();
            var rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, Vector2.zero,
                new Vector2(cellWidth, cellHeight), Vector2.zero);
            foreach (var rect in rects)
            {
                var metaData = new SpriteMetaData
                {
                    name = texture.name + "_" + Array.IndexOf(rects, rect),
                    rect = rect,
                    alignment = 0,
                    pivot = Vector2.zero
                };
                spriteMetaDatas.Add(metaData);
            }

            var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (textureImporter == null)
            {
                Debug.LogError("Texture importer is null.");
                return;
            }

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = spriteMetaDatas.Count > 1 ? SpriteImportMode.Multiple : SpriteImportMode.Single;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.spritePixelsPerUnit = 32;
            textureImporter.filterMode = FilterMode.Point;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.spritePivot = new Vector2(spritePivot.x / cellWidth, spritePivot.y / cellHeight);

            var textureImportSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureImportSettings);
            textureImportSettings.spriteMeshType = SpriteMeshType.FullRect;
            textureImporter.SetTextureSettings(textureImportSettings);

            textureImporter.spritesheet = spriteMetaDatas.ToArray();
            AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        }

        private void SaveSpritesToAsset(SerializedObject _object)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath).OfType<Sprite>().ToArray();
            var spritesArray = _object.FindProperty("sprites");
            spritesArray.arraySize = sprites.Length;
            for (var i = 0; i < sprites.Length; i++)
            {
                spritesArray.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
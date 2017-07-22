using System;
using System.IO;
using UnityEditor;
using UnityEngine;
// ReSharper disable CheckNamespace

namespace ScriptNamespaceProcessor
{
	public class ScriptNamespaceProcessor : UnityEditor.AssetModificationProcessor 
	{

        private static void OnWillCreateAsset(string _path)
        {
            // Remove the .meta from path since this function only applies to meta files, however the normal .cs file will still be created along side.
            _path = _path.Replace(".meta", "");
            var index = _path.IndexOf(".", StringComparison.Ordinal);

            if (index < 0) return;

            var file = _path.Substring(index);

            // only want to modify files that are .cs
            if (file != ".cs") return;

            // Create the full file path relative to the system directory, rather than the local unity path (E.g. Assets/Scripts to C:/blah blah.
            index = Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal);
            _path = Application.dataPath.Substring(0, index) + _path;

            // Ensure the file actually exists..
            if (!File.Exists(_path)) return;

            // Store the original contents of the file, and replace the #NAMESPACE# phrase in the generated script template with the actual file namespace 
            var fileContent = File.ReadAllText(_path);
            var fileNamespace = GetNameSpaceFromPath(_path);
            fileContent = fileContent.Replace("#NAMESPACE#", fileNamespace);

            // Write the file contents with the added namespace back to the file.
            File.WriteAllText(_path, fileContent);

            // Notifiy unity it needs to reimport the modified file.
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        /// <summary>
        /// Returns a script namespace from its given file path.
        /// </summary>
        /// <param name="_path">System directory to file.</param>
        /// <returns></returns>
        private static string GetNameSpaceFromPath(string _path)
        {
            // Trim the path down to only the Assets/... directory
            var assetsRootIndex = _path.IndexOf("Assets", StringComparison.Ordinal);
            var assetsRoot = _path.Substring(assetsRootIndex);

            // Remove the file name from the namespace
            var lastSlashIndex = assetsRoot.LastIndexOf("/", StringComparison.Ordinal);
            assetsRoot = assetsRoot.Remove(lastSlashIndex);

            // Replace all "/" in directory with "." for namespace
            var fileNamespace = assetsRoot.Replace("/", ".");

            return fileNamespace;
        }
	}
}

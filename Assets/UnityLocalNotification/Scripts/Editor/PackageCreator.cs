using System.Collections.Generic;
using UnityEditor;

namespace UnityLocalNotification.Scripts.Editor
{
    public class PackageCreator
    {
        private static readonly string[] AssetList = AssetDatabase.FindAssets(
            "*", new[]
            {
                "Assets/UnityLocalNotification/Plugins",
                "Assets/UnityLocalNotification/Scripts",
            });

        [MenuItem("Assets/Export Package")]
        public static void ExportPackage()
        {
            var assetList = new List<string>();

            foreach (var guid in AssetList)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                assetList.Add(assetPath);
            }

            AssetDatabase.ExportPackage(assetList.ToArray(), "unity_local_notification.unitypackage",
                ExportPackageOptions.Recurse);
        }
    }
}
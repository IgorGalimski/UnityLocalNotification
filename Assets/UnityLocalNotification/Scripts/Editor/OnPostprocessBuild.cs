#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace UnityLocalNotification
{
    public class OnPostprocessBuild
    {
        private static string ICONS_PROJECT_PATH = "Assets/UnityLocalNotification/Icons/iOS";
        
        [PostProcessBuild]
        public static void OnPostprocessBuildHandler(BuildTarget buildTarget, string path)
        {
            AddFrameworks(path);
            CopyIcons(path);
        }

        private static void AddFrameworks(string path)
        {
            var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
   
            var proj = new PBXProject ();
            proj.ReadFromString (File.ReadAllText (projPath));
   
            var target = proj.GetUnityFrameworkTargetGuid();

            proj.AddFrameworkToProject(target, "UserNotifications.framework", true);
            
            proj.AddCapability(target, PBXCapabilityType.PushNotifications);
            File.WriteAllText (projPath, proj.WriteToString());
            
            var capabilities =
                new ProjectCapabilityManager(PBXProject.GetPBXProjectPath(path), "app.entitlements", "Unity-iPhone");
            capabilities.AddPushNotifications(true);
            capabilities.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            capabilities.WriteToFile();
        }

        private static void CopyIcons(string path)
        {
            if (!Directory.Exists(ICONS_PROJECT_PATH))
            {
                return;
            }
            
            foreach (var file in Directory.GetFiles(ICONS_PROJECT_PATH, "*.png", 
                SearchOption.AllDirectories))
            {
                File.Copy(file, Path.Combine(path , Path.GetFileName(file)));
            }
        }
    }
}

#endif
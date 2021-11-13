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

            var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
   
            var proj = new PBXProject ();
            proj.ReadFromString (File.ReadAllText (projPath));

            var target = proj.GetUnityMainTargetGuid();

            proj.AddFrameworkToProject(target, "UserNotifications.framework", true);
            
            proj.AddCapability(target, PBXCapabilityType.PushNotifications);
            
            if (Directory.Exists(ICONS_PROJECT_PATH))
            {
                foreach (var file in Directory.GetFiles(ICONS_PROJECT_PATH, "*.png", 
                    SearchOption.AllDirectories))
                {
                    var dstLocalPath = Path.GetFileName(file);
                    var resourcesBuildPhase = proj.GetResourcesBuildPhaseByTarget(target);
                    var fileRef = proj.AddFile(dstLocalPath, dstLocalPath);
                    
                    File.Copy(file, Path.Combine(path, dstLocalPath), true);
                    proj.AddFileToBuild(target, fileRef);
                }
            }

            File.WriteAllText (projPath, proj.WriteToString());
            
            var capabilities =
                new ProjectCapabilityManager(PBXProject.GetPBXProjectPath(path), "app.entitlements", "Unity-iPhone");
            capabilities.AddPushNotifications(true);
            capabilities.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            capabilities.WriteToFile();
        }
    }
}

#endif
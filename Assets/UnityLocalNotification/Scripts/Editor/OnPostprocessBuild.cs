#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace UnityLocalNotification
{
    public class OnPostprocessBuild
    {
        [PostProcessBuild]
        public static void OnPostprocessBuildHandler(BuildTarget buildTarget, string path)
        {
            var projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
   
            var proj = new PBXProject ();
            proj.ReadFromString (File.ReadAllText (projPath));
   
            var target = proj.TargetGuidByName ("Unity-iPhone");

            proj.AddFrameworkToProject(target, "UserNotifications.framework", true);
            
            proj.AddCapability(target, PBXCapabilityType.PushNotifications);
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
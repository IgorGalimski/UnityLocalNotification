using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEditor.Android;
using UnityEngine;

namespace UnityLocalNotification.Scripts.Editor
{
    public class GradlePostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        private const string ANDROID_NAMESPACE_URI = "http://schemas.android.com/apk/res/android";
        private const string NOTIFICATION_BROADCAST_RECEIVER = "com.igorgalimski.unitylocalnotification.NotificationBroadcastReceiver";
        
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log("Bulid path : " + path);
            
            UpdateBuildGradle(path);
            UpdateAndroidManifest(path);
        }

        private void UpdateBuildGradle(string path)
        {
            var buildGradleFile = path + "/build.gradle";

            var gradle = File.ReadAllLines(buildGradleFile).ToList();

            var dependenciesIndex = Array.FindIndex(gradle.ToArray(), x => x == "dependencies {");
            if (dependenciesIndex != -1)
            {
                gradle.Insert(dependenciesIndex + 1, "    implementation 'androidx.appcompat:appcompat:1.2.0'");

                File.Delete(buildGradleFile);
                
                File.WriteAllLines(buildGradleFile, gradle);
            }
        }

        private void UpdateAndroidManifest(string path)
        {
            var manifestPath = path + "/src/main/AndroidManifest.xml";
            
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);
            
            var applicationXmlNode = manifestDoc.SelectSingleNode("manifest/application");
            
            var notificationManagerReceiver = manifestDoc.CreateElement("receiver");
            notificationManagerReceiver.SetAttribute("name", ANDROID_NAMESPACE_URI, NOTIFICATION_BROADCAST_RECEIVER);
            notificationManagerReceiver.SetAttribute("enabled", ANDROID_NAMESPACE_URI, "false");

            var filter = manifestDoc.CreateElement("intent-filter");
            notificationManagerReceiver.AppendChild(filter);
            
            var action = manifestDoc.CreateElement("action");
            action.SetAttribute("name", ANDROID_NAMESPACE_URI, "android.intent.action.BOOT_COMPLETED");
            filter.AppendChild(action);

            applicationXmlNode.AppendChild(notificationManagerReceiver);
            
            var bootCompletedAction = manifestDoc.CreateElement("uses-permission");
            bootCompletedAction.SetAttribute("name", ANDROID_NAMESPACE_URI, "android.permission.RECEIVE_BOOT_COMPLETED");
            
            var manifestXmlNode = manifestDoc.SelectSingleNode("manifest");
            manifestXmlNode.AppendChild(bootCompletedAction);
            
            manifestDoc.Save(manifestPath);
        }
    }
}

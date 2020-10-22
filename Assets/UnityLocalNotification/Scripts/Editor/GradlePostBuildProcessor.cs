using System;
using System.IO;
using System.Linq;
using System.Xml;
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

            applicationXmlNode.AppendChild(notificationManagerReceiver);
            
            manifestDoc.Save(manifestPath);
        }
    }
}

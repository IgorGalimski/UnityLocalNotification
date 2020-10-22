using System;
using System.IO;
using System.Linq;
using UnityEditor.Android;
using UnityEngine;

namespace UnityLocalNotification.Scripts.Editor
{
    public class GradlePostBuildProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log("Bulid path : " + path);
            
            UpdateBuildGradle(path);
        }

        private void UpdateBuildGradle(string path)
        {
            
            string buildGradleFile = path + "/build.gradle";

            var gradle = File.ReadAllLines(buildGradleFile).ToList();

            var dependenciesIndex = Array.FindIndex(gradle.ToArray(), x => x == "dependencies {");
            if (dependenciesIndex != -1)
            {
                gradle.Insert(dependenciesIndex + 1, "    implementation 'androidx.appcompat:appcompat:1.2.0'");

                File.Delete(buildGradleFile);
                
                File.WriteAllLines(buildGradleFile, gradle);
            }
        }
    }
}

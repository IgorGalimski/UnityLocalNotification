using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace UnityLocalNotification.Scripts.Editor
{
    public class TexturePostProcessor : IPostGenerateGradleAndroidProject
    {
        private const string SMALL_TEXTURE_PATH = "Assets/icon_0.png";
        private const string LARGE_TEXTURE_PATH = "Assets/icon_1.png";

        [Flags]
        private enum TextureScale : byte
        {
            Small = 1,
            Large = 2,
            All = Small | Large
        }

        private class TextureInfo
        {
            public int Dimension { get; }
            public string FileName { get; }

            public TextureScale TextureScale { get; }

            public TextureInfo(int dimension, string fileName, TextureScale textureScale)
            {
                Dimension = dimension;
                FileName = fileName;
                TextureScale = textureScale;
            }
        }

        private static readonly List<TextureInfo> _textures = new List<TextureInfo>
        {
            new TextureInfo(192, "drawable-xxhdpi-v11/{0}.png", TextureScale.Large),
            new TextureInfo(128, "drawable-xhdpi-v11/{0}.png", TextureScale.All),
            new TextureInfo(96, "drawable-hdpi-v11/{0}.png", TextureScale.All),
            new TextureInfo(64, "drawable-mdpi-v11/{0}.png", TextureScale.All),
            new TextureInfo(48, "drawable-ldpi-v11/{0}.png", TextureScale.All)
        };

        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            DownscaleTextures(path, SMALL_TEXTURE_PATH, TextureScale.Small);
            DownscaleTextures(path, LARGE_TEXTURE_PATH, TextureScale.Large);
        }

        private void DownscaleTextures(string projectPath, string texturePath, TextureScale textureScale)
        {
            if (!File.Exists(texturePath)) return;

            var texture = (Texture2D) AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
            var textureName = Path.GetFileNameWithoutExtension(texturePath);

            foreach (var textureInfo in _textures)
                if (textureInfo.TextureScale.HasFlag(textureScale))
                {
                    var textureCopy = CopyTexture(texture,
                        (int) (textureInfo.Dimension * (textureScale == TextureScale.Large ? 1f : 0.375f)));
                    var processedTexture = ProcessTextureForType(textureCopy, textureScale);

                    SaveTexture(projectPath, string.Format(textureInfo.FileName, textureName),
                        processedTexture.EncodeToPNG());
                }
        }

        private Texture2D CopyTexture(Texture2D sourceTexture, int dimension)
        {
            if (sourceTexture == null)
                return null;

            var assetPath = AssetDatabase.GetAssetPath(sourceTexture);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            var readable = importer.isReadable;
            if (!readable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();

                AssetDatabase.ImportAsset(assetPath);
            }

            var newTexture = new Texture2D(dimension, dimension, TextureFormat.ARGB32, false);

            var destPixels = new Color[dimension * dimension];
            for (var y = 0; y < dimension; ++y)
            for (var x = 0; x < dimension; ++x)
                destPixels[y * dimension + x] =
                    sourceTexture.GetPixelBilinear((float) x / (float) dimension, (float) y / (float) dimension);
            newTexture.SetPixels(destPixels);
            newTexture.Apply();

            if (!readable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();

                AssetDatabase.ImportAsset(assetPath);
            }

            return newTexture;
        }

        private Texture2D ProcessTextureForType(Texture2D sourceTexture, TextureScale textureScale)
        {
            Texture2D texture;
            if (textureScale.HasFlag(TextureScale.Small))
            {
                texture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, true, false);
                for (var i = 0; i < sourceTexture.mipmapCount; i++)
                {
                    var c_0 = sourceTexture.GetPixels(i);
                    var c_1 = texture.GetPixels(i);
                    for (var i1 = 0; i1 < c_0.Length; i1++)
                    {
                        var a = c_0[i1].r + c_0[i1].g + c_0[i1].b;
                        c_1[i1].r = c_1[i1].g = c_1[i1].b = a > 127 ? 0 : 1;
                        c_1[i1].a = c_0[i1].a;
                    }

                    texture.SetPixels(c_1, i);
                }

                texture.Apply();
            }
            else
            {
                texture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, true);
                texture.SetPixels(sourceTexture.GetPixels());
                texture.Apply();
            }

            return texture;
        }

        private void SaveTexture(string projectPath, string textureName, byte[] textureData)
        {
            if (!Directory.Exists(Path.Combine(projectPath, "src")))
                projectPath = Path.Combine(projectPath, PlayerSettings.productName);

            var fileInfo = new FileInfo($"{projectPath}/src/main/res/{textureName}");
            if (fileInfo.Directory != null)
            {
                fileInfo.Directory.Create();
                File.WriteAllBytes(fileInfo.FullName, textureData);
            }
        }
    }
}
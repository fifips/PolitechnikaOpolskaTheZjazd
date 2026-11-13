using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheZjazd.ExamSupervisor.EditorTools
{
    /// <summary>
    /// Draws the ruler and the three "swish" frames as pixel art and saves them as PNG files.
    /// Existing files are left alone, so hand-drawn replacements survive a scene rebuild.
    /// </summary>
    public static class RulerSpriteGenerator
    {
        public const string Folder = "Assets/Sprites/Linijka";
        public const string RulerPath = Folder + "/linijka.png";
        public const int SwishFrameCount = 3;
        public static string SwishPath(int index) => Folder + "/swist_" + (index + 1) + ".png";

        public static void EnsureSprites()
        {
            Directory.CreateDirectory(Folder);
            bool created = false;

            if (!File.Exists(RulerPath)) { Save(RulerPath, DrawRuler()); created = true; }
            for (int i = 0; i < SwishFrameCount; i++)
                if (!File.Exists(SwishPath(i))) { Save(SwishPath(i), DrawSwish(i)); created = true; }

            if (created) AssetDatabase.Refresh();

            ConfigureImport(RulerPath, new Vector2(0f, 0.5f)); // pivot on the handle (left, middle)
            for (int i = 0; i < SwishFrameCount; i++)
                ConfigureImport(SwishPath(i), new Vector2(0.5f, 0.5f));
        }

        // 16 x 4 px wooden ruler: lighter top edge, dark outline, tick marks along the bottom.
        static Texture2D DrawRuler()
        {
            var tex = NewTexture(16, 4);
            var wood = new Color32(212, 170, 100, 255);
            var woodLight = new Color32(235, 205, 140, 255);
            var outline = new Color32(110, 72, 36, 255);
            var tick = new Color32(70, 45, 25, 255);

            for (int x = 0; x < 16; x++)
            {
                tex.SetPixel(x, 3, woodLight);
                tex.SetPixel(x, 2, wood);
                tex.SetPixel(x, 1, wood);
                tex.SetPixel(x, 0, outline);
            }
            for (int x = 1; x < 16; x += 2)
                tex.SetPixel(x, 1, tick);       // short ticks
            for (int x = 3; x < 16; x += 4)
                tex.SetPixel(x, 2, tick);       // long ticks
            for (int y = 0; y < 4; y++)
                tex.SetPixel(15, y, outline);   // end cap
            tex.SetPixel(0, 3, outline);
            tex.SetPixel(0, 0, outline);        // handle end: rounded corners
            tex.Apply();
            return tex;
        }

        // 16 x 16 px crescent pointing right (+x). Later frames are longer and more transparent.
        static Texture2D DrawSwish(int frame)
        {
            var tex = NewTexture(16, 16);
            float[] halfSpanDegrees = { 20f, 38f, 55f };
            float[] alpha = { 1f, 0.8f, 0.5f };
            var centre = new Vector2(-3.5f, 7.5f);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    var offset = new Vector2(x, y) - centre;
                    float distance = offset.magnitude;
                    float angle = Mathf.Abs(Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg);
                    if (Mathf.Abs(distance - 11f) < 1.2f && angle < halfSpanDegrees[frame])
                        tex.SetPixel(x, y, new Color(1f, 0.95f, 0.75f, alpha[frame]));
                }
            }
            tex.Apply();
            return tex;
        }

        static Texture2D NewTexture(int width, int height)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var clear = new Color32[width * height];
            tex.SetPixels32(clear);
            return tex;
        }

        static void Save(string path, Texture2D tex)
        {
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }

        static void ConfigureImport(string path, Vector2 pivot)
        {
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = pivot;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;

public class ArtistFont : MonoBehaviour
{
    //public static void BatchCreateArtistFont()
    //{
    //    string dirName = "";
    //    string fntname = EditorUtils.SelectObjectPathInfo(ref dirName).Split('.')[0];
    //    Debug.Log(fntname);
    //    Debug.Log(dirName);

    //    string fntFileName = dirName + fntname + ".fnt";

    //    Font CustomFont = new Font();
    //    {
    //        AssetDatabase.CreateAsset(CustomFont, dirName + fntname + ".fontsettings");
    //        AssetDatabase.SaveAssets();
    //    }

    //    TextAsset BMFontText = null;
    //    {
    //        BMFontText = AssetDatabase.LoadAssetAtPath(fntFileName, typeof(TextAsset)) as TextAsset;
    //    }

    //    BMFont mbFont = new BMFont();
    //    BMFontReader.Load(mbFont, BMFontText.name, BMFontText.bytes);  // 借用NGUI封装的读取类
    //    CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
    //    for (int i = 0; i < mbFont.glyphs.Count; i++)
    //    {
    //        BMGlyph bmInfo = mbFont.glyphs[i];
    //        CharacterInfo info = new CharacterInfo();
    //        info.index = bmInfo.index;
    //        float uvx = 1f * bmInfo.x / mbFont.texWidth;
    //        float uvy = 1 - (1f * bmInfo.y / mbFont.texHeight);
    //        float uvw = 1f * bmInfo.width / mbFont.texWidth;
    //        float uvh = -1f * bmInfo.height / mbFont.texHeight;
    //        info.uvBottomLeft = new Vector2(uvx, uvy);
    //        info.uvBottomRight = new Vector2(uvx + uvw, uvy);
    //        info.uvTopLeft = new Vector2(uvx, uvy + uvh);
    //        info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);
    //        info.minX = bmInfo.offsetX;
    //        info.minY = bmInfo.offsetY + bmInfo.height / 2;   // 这样调出来的效果是ok的，原理未知  
    //        info.glyphWidth = bmInfo.width;
    //        info.glyphHeight = -bmInfo.height; // 同上，不知道为什么要用负的，可能跟unity纹理uv有关  
    //        info.advance = bmInfo.advance;
    //        characterInfo[i] = info;
    //    }
    //    CustomFont.characterInfo = characterInfo;

    //    string textureFilename = dirName + mbFont.spriteName + ".png";
    //    Material mat = null;
    //    {
    //        Shader shader = Shader.Find("Transparent/Diffuse");
    //        mat = new Material(shader);
    //        Texture tex = AssetDatabase.LoadAssetAtPath(textureFilename, typeof(Texture)) as Texture;
    //        mat.SetTexture("_MainTex", tex);

    //        AssetDatabase.CreateAsset(mat, dirName + fntname + ".mat");
    //        AssetDatabase.SaveAssets();
    //    }
    //    CustomFont.material = mat;
    //    EditorUtility.SetDirty(CustomFont);
    //    AssetDatabase.SaveAssets();
    //    EditorUtility.DisplayDialog("完成", $"字体文件{BMFontText.name}生成完成！", "OK");
    //}

    internal static void BatchCreateArtistFont(string path)
    {
        string dirName = Path.GetDirectoryName(path);
        string fntname = Path.GetFileNameWithoutExtension(path);


        string fntFileName = path;
        Debug.Log(fntname);
        Debug.Log(dirName);
        Debug.Log(fntFileName);

        Font CustomFont = new Font();
        {
            AssetDatabase.CreateAsset(CustomFont, Path.Combine(dirName, fntname + ".fontsettings"));
            AssetDatabase.SaveAssets();
        }

        TextAsset BMFontText = null;
        {
            BMFontText = AssetDatabase.LoadAssetAtPath(fntFileName, typeof(TextAsset)) as TextAsset;
        }

        BMFont mbFont = new BMFont();
        //BMFontReader.Load(mbFont, BMFontText.name, BMFontText.bytes);  // 借用NGUI封装的读取类
        BMFontReader.Load(mbFont, BMFontText.name, File.ReadAllText(Path.GetFullPath(fntFileName)));   // 借用NGUI封装的读取类
        CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
        for (int i = 0; i < mbFont.glyphs.Count; i++)
        {
            BMGlyph bmInfo = mbFont.glyphs[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
            float uvx = 1f * bmInfo.x / mbFont.texWidth;
            float uvy = 1 - (1f * bmInfo.y / mbFont.texHeight);
            float uvw = 1f * bmInfo.width / mbFont.texWidth;
            float uvh = -1f * bmInfo.height / mbFont.texHeight;
            info.uvBottomLeft = new Vector2(uvx, uvy);
            info.uvBottomRight = new Vector2(uvx + uvw, uvy);
            info.uvTopLeft = new Vector2(uvx, uvy + uvh);
            info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);
            info.minX = bmInfo.offsetX;
            info.minY = bmInfo.offsetY + bmInfo.height / 2;   // 这样调出来的效果是ok的，原理未知  
            info.glyphWidth = bmInfo.width;
            info.glyphHeight = -bmInfo.height; // 同上，不知道为什么要用负的，可能跟unity纹理uv有关  
            info.advance = bmInfo.advance;
            characterInfo[i] = info;
        }
        CustomFont.characterInfo = characterInfo;
        Debug.LogError(mbFont.spriteName);
        string textureFilename = Path.Combine(dirName, mbFont.spriteName + ".png");
        Material mat = null;
        {
            Shader shader = Shader.Find("Transparent/Diffuse");
            mat = new Material(shader);
            Texture tex = AssetDatabase.LoadAssetAtPath(textureFilename, typeof(Texture)) as Texture;
            mat.SetTexture("_MainTex", tex);

            AssetDatabase.CreateAsset(mat, Path.Combine(dirName, fntname + ".mat"));
            AssetDatabase.SaveAssets();
        }
        CustomFont.material = mat;
        EditorUtility.SetDirty(CustomFont);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("完成", $"字体文件{BMFontText.name}生成完成！", "OK");
    }
}

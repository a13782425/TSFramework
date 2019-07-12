using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class FontEditor : EditorWindow
{
    [MenuItem("SSFrame/Font/创建UGUI字体 #F", false, 101)]
    public static void OpenWindow()
    {
        EditorWindow.GetWindowWithRect<FontEditor>(new Rect(100, 100, 400, 200));
    }

    private TextAsset textAsset;
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("使用BMFont导出Text或XML格式\r\n字体文件需和图片文件同目录", new GUIStyle() { fontSize = 20 });
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("Box");
        textAsset = EditorGUILayout.ObjectField(new GUIContent("字体文件(非XML)："), textAsset, typeof(TextAsset), false) as TextAsset;
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("导出字体"))
        {
            if (textAsset != null)
            {
                Debug.LogError(AssetDatabase.GetAssetPath(textAsset));
                ArtistFont.BatchCreateArtistFont(AssetDatabase.GetAssetPath(textAsset));
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "没有选中字体文件！", "OK");
            }

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
    }
}


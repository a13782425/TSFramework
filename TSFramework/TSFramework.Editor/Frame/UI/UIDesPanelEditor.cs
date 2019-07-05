using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static TSFrame.Editor.Properties.Resources;
public sealed class UIDesPanelEditor : EditorWindow
{
    [MenuItem("TSFrame/UI/帮助", false, 100)]
    public static void OpenWindow()
    {
        EditorWindow.GetWindowWithRect<UIDesPanelEditor>(new Rect(100, 100, 600, 200));
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(UIDescription, new GUIStyle() { fontSize = 20 });
        EditorGUILayout.EndVertical();

    }
}


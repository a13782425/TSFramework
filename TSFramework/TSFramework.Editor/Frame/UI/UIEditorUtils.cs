using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static TSFrame.Editor.Properties.Resources;

internal static class UIEditorUtils
{
    internal readonly static string _configFilePath = Application.dataPath + "/../config";
    internal const string TAG_NAME = "Export";
    public static Dictionary<string, Type> PrefixTypeDic = new Dictionary<string, Type>()
    {
        {"btn_", typeof(Button) },
        {"img_", typeof(Image) },
        {"txt_", typeof(Text) },
        {"inp_", typeof(InputField) },
        {"srect_", typeof(ScrollRect) },
        {"sbar_", typeof(Scrollbar) },
        {"go_", typeof(GameObject) },
        {"tran_", typeof(Transform) },
        {"rtran_", typeof(RectTransform) },
        {"tog_", typeof(Toggle) },
        {"sli_", typeof(Slider) },
        {"drop_", typeof(Dropdown) },
        {"can_", typeof(Canvas) },
    };

    [MenuItem("TSFrame/UI/生成全部", false, 1500)]
    internal static void GenerateAll()
    {
        PanelEditor.GeneratePanel();
        ItemEditor.GenerateItem();
    }

    internal static bool CheckUIConfig()
    {
        if (!File.Exists(_configFilePath))
        {
            First();
        }
        IniTool iniTool = new IniTool();
        iniTool.Open(_configFilePath);
        string panelPath = iniTool.ReadValue("UI", "PanelPath", "");
        string itemPath = iniTool.ReadValue("UI", "ItemPath", "");
        string panelScriptPath = iniTool.ReadValue("UIScript", "PanelScriptPath", "");
        string itemScriptPath = iniTool.ReadValue("UIScript", "ItemScriptPath", "");

        if (string.IsNullOrWhiteSpace(panelPath) || string.IsNullOrWhiteSpace(itemPath) || string.IsNullOrWhiteSpace(panelScriptPath) || string.IsNullOrWhiteSpace(itemScriptPath))
        {
            return false;
        }
        return true;
    }

    private static void First()
    {
        File.Create(_configFilePath).Dispose();
        File.WriteAllText(_configFilePath, ConfigTemplate);
        InitTag();
    }

    private static void InitTag()
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        // Tags Property
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        //Debug.Log("TagsPorp Size:" + tagsProp.arraySize);
        List<string> tags = new List<string>();
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            tags.Add(tagsProp.GetArrayElementAtIndex(i).stringValue);
        }

        if (!tags.Contains(TAG_NAME))
        {
            tags.Add(TAG_NAME);
            tagsProp.ClearArray();

            tagManager.ApplyModifiedProperties();


            for (int i = 0; i < tags.Count; i++)
            {
                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(i);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
                // Set array element to tagName
                sp.stringValue = tags[i];

                tagManager.ApplyModifiedProperties();
            }
        }
    }
}


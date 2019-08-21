using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSFrame;
using UnityEditor;
using UnityEngine;
using static TSFrame.Editor.Properties.Resources;

internal static class PanelEditor
{
    /// <summary>
    /// Resources读取路径
    /// </summary>
    internal static string _panelUIResPath = null;
    /// <summary>
    /// UI在Asset的路径
    /// </summary>
    internal static string _panelUIPath = null;
    /// <summary>
    /// 代码所在路径
    /// </summary>
    internal static string _panelCodePath = null;

    internal static IniTool _iniTool = null;

    //internal static FieldInfo _injectModelField = null;

    [MenuItem("TSFrame/UI/生成Panel #P", false, 0)]
    internal static void GeneratePanel()
    {
        try
        {
            if (!UIEditorUtils.CheckUIConfig())
            {
                EditorUtility.DisplayDialog("错误", "配置表路径没有配置，请配置与Asset同级的config文件", "OK");
                return;
            }
            InitData();

            if (!Directory.Exists(_panelCodePath))
            {
                Directory.CreateDirectory(_panelCodePath);
            }
            EditorUtility.DisplayProgressBar("生成Panel", "正在生成Panel", 0);
            UIEditorUtils.ErrorList = new List<string>();
            Thread.Sleep(200);
            string[] strs = Directory.GetFiles(Path.Combine(Application.dataPath, _panelUIPath), "*.prefab");
            float length = strs.Length;
            float index = 0;
            foreach (var item in strs)
            {
                index++;
                //_injectModelField = null;
                EditorUtility.DisplayProgressBar("生成Panel", $"正在生成{Path.GetFileNameWithoutExtension(item)}", index / length);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets", _panelUIPath, Path.GetFileName(item)));
                List<TranDto> trans = new List<TranDto>();
                UIEditorUtils.GetTrans(obj.transform, "", trans);
                trans.Reverse();
                GenerateUserCode(obj.name);
                GenerateCode(obj.name + ".gen.cs", PanelTemplate, trans, _panelCodePath, Path.Combine(_panelUIResPath, Path.GetFileNameWithoutExtension(item)));
                Thread.Sleep(200);
            }
            EditorUtility.DisplayProgressBar("生成Panel", "生成Panel完成", 1);
            Thread.Sleep(200);
            EditorUtility.ClearProgressBar();
            if (UIEditorUtils.ErrorList.Count > 0)
            {
                foreach (var item in UIEditorUtils.ErrorList)
                {
                    EditorUtility.DisplayDialog("警告", item, "OK");
                }
            }
            AssetDatabase.Refresh();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        finally
        {
            if (_iniTool != null)
            {
                _iniTool.Close();
            }
            _iniTool = null;
            //_injectModelField = null;
            EditorUtility.ClearProgressBar();
        }
    }

    private static void GenerateUserCode(string name)
    {
        string panelPath = _iniTool.ReadValue("UIScript", "PanelScriptPath", "");
        string panelModelPath = _iniTool.ReadValue("UIScript", "PanelDataModelPath", "");
        string modelName = "";
        if (!string.IsNullOrWhiteSpace(panelModelPath))
        {
            modelName = name + "Data";
            panelModelPath = Path.Combine(Application.dataPath, panelModelPath, modelName + ".cs");
            if (!File.Exists(panelModelPath))
            {
                string temp = ModelTemplate;
                temp = temp.Replace("{ClassName}", modelName);
                File.WriteAllText(panelModelPath, temp, new UTF8Encoding());
            }
        }
        if (!string.IsNullOrWhiteSpace(panelPath))
        {
            panelPath = Path.Combine(Application.dataPath, panelPath, name + ".cs");
            if (!File.Exists(panelPath))
            {
                if (string.IsNullOrWhiteSpace(modelName))
                {
                    modelName = "NullModel";
                }
                string modelFieldName = modelName;
                string tempChar = modelFieldName[0].ToString();
                tempChar = tempChar.ToLower();
                modelFieldName = modelFieldName.Remove(0, 1);
                modelFieldName = modelFieldName.Insert(0, tempChar);
                string temp = UITemplate;
                temp = temp.Replace("{ClassName}", name);
                temp = temp.Replace("{ModelClass}", modelName);
                temp = temp.Replace("{ModelClassName}", modelFieldName);
                File.WriteAllText(panelPath, temp, new UTF8Encoding());
            }
        }
    }

    private static void GenerateCode(string scriptName, string templateText, List<TranDto> trans, string panelCodePath, string uiPath)
    {
        uiPath = uiPath.Replace(@"\", "/");
        string temp = templateText;
        string varCode = UIEditorUtils.GetVarCode(trans, _iniTool);
        string componentCode = UIEditorUtils.GetComponentCode(trans, _iniTool);
        FieldInfo injectField = null;
        string injectData = UIEditorUtils.GetInjectModelField(UIEditorUtils.GetTypeForPanel(Path.GetFileName(uiPath)), ref injectField);
        string bindingData = UIEditorUtils.GetBindingData(UIEditorUtils.GetTypeForPanel(Path.GetFileName(uiPath)), injectField);

        temp = temp.Replace("{GeneratedTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        temp = temp.Replace("{UIPath}", uiPath);
        temp = temp.Replace("{ClassName}", Path.GetFileName(uiPath));
        temp = temp.Replace("{InjectData}", injectData);
        temp = temp.Replace("{BindData}", bindingData);
        temp = temp.Replace("{Variable}", varCode);
        temp = temp.Replace("{BindComponent}", componentCode);

        File.WriteAllText(Path.Combine(panelCodePath, scriptName), temp, new UTF8Encoding());
    }

    private static void InitData()
    {
        _iniTool = new IniTool();
        _iniTool.Open(UIEditorUtils._configFilePath);
        _panelUIPath = _iniTool.ReadValue("UI", "PanelPath", "");
        _panelCodePath = _iniTool.ReadValue("UIScript", "PanelGeneratedScriptPath", "");
        _panelCodePath = Path.Combine(Application.dataPath, _panelCodePath);
        int index = _panelUIPath.IndexOf("Resources");
        _panelUIResPath = _panelUIPath.Substring(index + UIEditorUtils.RESOURCES_LENGTH, _panelUIPath.Length - index - UIEditorUtils.RESOURCES_LENGTH);
    }
}


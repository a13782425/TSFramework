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

internal static class ItemEditor
{
    /// <summary>
    /// Resources读取路径
    /// </summary>
    internal static string _itemUIResPath = null;
    /// <summary>
    /// UI在Asset的路径
    /// </summary>
    internal static string _itemUIPath = null;
    /// <summary>
    /// 代码所在路径
    /// </summary>
    internal static string _itemCodePath = null;

    internal static IniTool _iniTool = null;

    [MenuItem("TSFrame/UI/生成Item #I", false, 1)]
    internal static void GenerateItem()
    {
        try
        {
            if (!UIEditorUtils.CheckUIConfig())
            {
                EditorUtility.DisplayDialog("错误", "配置表路径没有配置，请配置与Asset同级的config文件", "OK");
                return;
            }
            InitData();

            if (!Directory.Exists(_itemCodePath))
            {
                Directory.CreateDirectory(_itemCodePath);
            }
            EditorUtility.DisplayProgressBar("生成Item", "正在生成Item", 0);
            UIEditorUtils.ErrorList = new List<string>();
            Thread.Sleep(200);
            string[] strs = Directory.GetFiles(Path.Combine(Application.dataPath, _itemUIPath), "*.prefab");
            float length = strs.Length;
            float index = 0;
            foreach (var item in strs)
            {
                index++;
                EditorUtility.DisplayProgressBar("生成Item", $"正在生成{Path.GetFileNameWithoutExtension(item)}", index / length);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets", _itemUIPath, Path.GetFileName(item)));
                List<TranDto> trans = new List<TranDto>();
                UIEditorUtils.GetTrans(obj.transform, "", trans);
                trans.Reverse();
                GenerateUserCode(obj.name);
                GenerateCode(obj.name + ".gen.cs", ItemTemplate, trans, _itemCodePath, Path.Combine(_itemUIResPath, Path.GetFileNameWithoutExtension(item)));
                Thread.Sleep(200);
            }
            EditorUtility.DisplayProgressBar("生成Item", "生成Item完成", 1);
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
            EditorUtility.ClearProgressBar();
        }
    }

    private static void GenerateUserCode(string name)
    {
        string itemPath = _iniTool.ReadValue("UIScript", "ItemScriptPath", "");
        string itemModelPath = _iniTool.ReadValue("UIScript", "ItemDataModelPath", "");
        string modelName = "";
        if (!string.IsNullOrWhiteSpace(itemModelPath))
        {
            modelName = name + "Data";
            itemModelPath = Path.Combine(Application.dataPath, itemModelPath, modelName + ".cs");
            if (!File.Exists(itemModelPath))
            {
                string temp = ModelTemplate;
                temp = temp.Replace("{ClassName}", modelName);
                File.WriteAllText(itemModelPath, temp, new UTF8Encoding());
            }
        }
        if (!string.IsNullOrWhiteSpace(itemPath))
        {
            itemPath = Path.Combine(Application.dataPath, itemPath, name + ".cs");
            if (!File.Exists(itemPath))
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
                File.WriteAllText(itemPath, temp, new UTF8Encoding());
            }
        }
    }

    private static void GenerateCode(string scriptName, string templateText, List<TranDto> trans, string itemCodePath, string uiPath)
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

        File.WriteAllText(Path.Combine(itemCodePath, scriptName), temp, new UTF8Encoding());
    }

    private static void InitData()
    {
        _iniTool = new IniTool();
        _iniTool.Open(UIEditorUtils._configFilePath);
        _itemUIPath = _iniTool.ReadValue("UI", "ItemPath", "");
        _itemCodePath = _iniTool.ReadValue("UIScript", "ItemGeneratedScriptPath", "");
        _itemCodePath = Path.Combine(Application.dataPath, _itemCodePath);
        int index = _itemUIPath.IndexOf("Resources");
        _itemUIResPath = _itemUIPath.Substring(index + UIEditorUtils.RESOURCES_LENGTH, _itemUIPath.Length - index - UIEditorUtils.RESOURCES_LENGTH);
    }
}

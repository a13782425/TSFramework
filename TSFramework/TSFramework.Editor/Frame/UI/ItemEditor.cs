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
                GenerateCode(obj.name + ".gen.cs", ItemTemplate, trans, _itemCodePath, Path.Combine(_itemUIResPath, Path.GetFileNameWithoutExtension(item)));
                Thread.Sleep(200);
            }
            EditorUtility.DisplayProgressBar("生成Item", "生成Item完成", 1);
            Thread.Sleep(200);
            EditorUtility.ClearProgressBar();
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
    //private static string GetBindingData(Type panelType)
    //{
    //    if (_injectModelField == null)
    //    {
    //        return "";
    //    }
    //    else
    //    {
    //        Type type = typeof(TSFrame.MVVM.BindableProperty<>);
    //        Type interfaceType = type.GetInterfaces()[0];
    //        Type bindType = typeof(TSFrame.BindingAttribute);
    //        Type elementType = typeof(TSFrame.UI.IElement);
    //        StringBuilder stringBuilder = new StringBuilder();
    //        FieldInfo[] panelFieldInfos = panelType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //        FieldInfo[] injectFieldInfos = _injectModelField.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //        foreach (var item in injectFieldInfos)
    //        {
    //            if (interfaceType.IsAssignableFrom(item.FieldType))
    //            {
    //                object[] objs = item.GetCustomAttributes(bindType, false);
    //                if (objs.Length == 1)
    //                {
    //                    BindingAttribute bindingAttribute = objs[0] as BindingAttribute;
    //                    string injectFieldName = item.Name;

    //                    foreach (var panelFieldInfo in panelFieldInfos)
    //                    {
    //                        if (elementType.IsAssignableFrom(panelFieldInfo.FieldType))
    //                        {
    //                            string element = panelFieldInfo.Name;
    //                            int index = element.IndexOf('_');
    //                            index++;
    //                            element = element.Substring(index, element.Length - index);
    //                            if (element.ToLower() == injectFieldName.ToLower())
    //                            {
    //                                if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OneWay) > 0)
    //                                {
    //                                    stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{_injectModelField.Name}.{injectFieldName}, this.{element}, BindingMode.OneWay);");
    //                                    stringBuilder.AppendLine();
    //                                }
    //                                if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OneWayToSource) > 0)
    //                                {
    //                                    stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{_injectModelField.Name}.{injectFieldName}, this.{element}, BindingMode.OneWayToSource);");
    //                                    stringBuilder.AppendLine();
    //                                }
    //                                if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OnTime) > 0)
    //                                {
    //                                    stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{_injectModelField.Name}.{injectFieldName}, this.{element}, BindingMode.OnTime);");
    //                                    stringBuilder.AppendLine();
    //                                }
    //                                if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.TwoWay) > 0)
    //                                {
    //                                    stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{_injectModelField.Name}.{injectFieldName}, this.{element}, BindingMode.TwoWay);");
    //                                    stringBuilder.AppendLine();
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return stringBuilder.ToString();
    //    }
    //}

    //private static string GetInjectData(Type type)
    //{
    //    if (type == null)
    //    {
    //        return "";
    //    }
    //    else
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    //        Type attrType = typeof(InjectAttribute);
    //        foreach (var fieldInfo in fieldInfos)
    //        {
    //            object[] obj = fieldInfo.GetCustomAttributes(attrType, false);
    //            if (obj.Length > 0)
    //            {
    //                if ((obj[0] as InjectAttribute) != null)
    //                {
    //                    sb.AppendLine($"		this.{fieldInfo.Name} = new {fieldInfo.FieldType}();");
    //                    if (_injectModelField == null)
    //                    {
    //                        _injectModelField = fieldInfo;
    //                    }
    //                    sb.AppendLine();
    //                }
    //            }
    //        }
    //        return sb.ToString();
    //    }
    //}

    //private static string GetComponentCode(List<TranDto> trans)
    //{
    //    StringBuilder sb = new StringBuilder();

    //    foreach (var item in trans)
    //    {
    //        string typeName = UIEditorUtils.GetExportType(item.Tran.name, _iniTool);
    //        string mvvmTypeName = UIEditorUtils.GetMVVMExportType(typeName);
    //        if (typeName == typeof(Transform).Name)
    //        {
    //            sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\");");
    //        }
    //        else if (typeName == typeof(GameObject).Name)
    //        {
    //            sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").gameObject;");
    //        }
    //        else
    //        {
    //            if (string.IsNullOrWhiteSpace(mvvmTypeName))
    //            {
    //                sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").GetComponent<{typeName}>();");
    //            }
    //            else
    //            {
    //                sb.AppendLine($"        this.raw_{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").GetComponent<{typeName}>();");
    //                sb.AppendLine();
    //                sb.AppendLine($"        this.BindingElement(this.raw_{item.Tran.name}, out this.{item.Tran.name});");
    //            }
    //        }
    //        sb.AppendLine();
    //    }

    //    return sb.ToString();
    //}

    //private static string GetVarCode(List<TranDto> trans)
    //{
    //    StringBuilder sb = new StringBuilder();

    //    foreach (var item in trans)
    //    {
    //        string typeName = UIEditorUtils.GetExportType(item.Tran.name, _iniTool);
    //        string mvvmTypeName = UIEditorUtils.GetMVVMExportType(typeName);

    //        if (string.IsNullOrWhiteSpace(mvvmTypeName))
    //        {
    //            sb.AppendLine($"    private {typeName} {item.Tran.name} = null;");
    //        }
    //        else
    //        {
    //            sb.AppendLine($"    private {typeName} raw_{item.Tran.name} = null;");
    //            sb.AppendLine();
    //            sb.AppendLine($"    private {mvvmTypeName} {item.Tran.name} = null;");
    //        }
    //        sb.AppendLine();
    //    }

    //    return sb.ToString();
    //}

    //private static void GetTrans(Transform transform, string parentPath, List<TranDto> trans)
    //{
    //    if (transform.childCount > 0)
    //    {
    //        for (int i = 0; i < transform.childCount; i++)
    //        {
    //            Transform tran = transform.GetChild(i);
    //            string path = parentPath;
    //            if (transform.parent != null)
    //            {
    //                path += (transform.name + "/");
    //            }
    //            GetTrans(tran, path, trans);
    //        }

    //    }
    //    if (transform.tag == UIEditorUtils.TAG_NAME)
    //    {
    //        TranDto tranDto = new TranDto();
    //        tranDto.Tran = transform;
    //        tranDto.ParentPath = parentPath;
    //        trans.Add(tranDto);
    //    }
    //}

    private static void InitData()
    {
        _iniTool = new IniTool();
        _iniTool.Open(UIEditorUtils._configFilePath);
        _itemUIPath = _iniTool.ReadValue("UI", "ItemPath", "");
        _itemCodePath = _iniTool.ReadValue("UIScript", "ItemScriptPath", "");
        _itemCodePath = Path.Combine(Application.dataPath, _itemCodePath);
        int index = _itemUIPath.IndexOf("Resources");
        _itemUIResPath = _itemUIPath.Substring(index + UIEditorUtils.RESOURCES_LENGTH, _itemUIPath.Length - index - UIEditorUtils.RESOURCES_LENGTH);
    }
}

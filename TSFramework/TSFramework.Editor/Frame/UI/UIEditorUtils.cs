using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSFrame;
using TSFrame.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static TSFrame.Editor.Properties.Resources;

internal static class UIEditorUtils
{
    internal readonly static string _configFilePath = Application.dataPath + "/../config";
    internal const string TAG_NAME = "Export";
    internal const string GAME_ASSEMBLY_FULLNAME = "Assembly-CSharp";
    internal const int RESOURCES_LENGTH = 10;
    internal readonly static Type _interfaceType = typeof(IElement);
    internal static List<string> ErrorList = new List<string>();
    public static Dictionary<string, string> MVVMTypeDic = new Dictionary<string, string>()
    {
        {"Button", "UIButton" },
        {"Image", "UIImage" },
        {"Text", "UIText" },
        {"InputField", "UIInputField" },
        {"TextPlus", "UITextPlus" },
        {"Toggle", "UIToggle" },
        {"Slider", "UISlider" },
        {"Scrollbar", "UIScrollbar" }
    };

    [MenuItem("TSFrame/UI/生成全部 &#U", false, 2)]
    internal static void GenerateAll()
    {
        PanelEditor.GeneratePanel();
        ItemEditor.GenerateItem();
    }

    /// <summary>
    /// 获取panel对应的类型
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    internal static Type GetTypeForPanel(string typeName)
    {
        Type returnType = null;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly item in assemblies)
        {
            Type[] types = item.GetTypes();
            foreach (var type in types)
            {
                if (type.Name == typeName && _interfaceType.IsAssignableFrom(type))
                {
                    returnType = type;
                    goto End;
                }
            }
        }

    End: return returnType;
    }

    internal static string GetExportType(string name, IniTool ini)
    {
        int index = name.IndexOf("_");
        string str = name.Substring(0, index + 1);
        string typeName = ini.ReadValue("UIExport", str, "");
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return typeof(GameObject).Name;
        }
        return typeName;
    }
    internal static string GetMVVMExportType(string raw_typeName)
    {
        if (MVVMTypeDic.ContainsKey(raw_typeName))
        {
            return MVVMTypeDic[raw_typeName];
        }
        return null;
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
        string panelScriptPath = iniTool.ReadValue("UIScript", "PanelGeneratedScriptPath", "");
        string itemScriptPath = iniTool.ReadValue("UIScript", "ItemGeneratedScriptPath", "");

        if (string.IsNullOrWhiteSpace(panelPath) || string.IsNullOrWhiteSpace(itemPath) || string.IsNullOrWhiteSpace(panelScriptPath) || string.IsNullOrWhiteSpace(itemScriptPath))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 生成绑定脚本
    /// </summary>
    /// <param name="panelType"></param>
    /// <param name="modelField"></param>
    /// <returns></returns>
    internal static string GetBindingData(Type panelType, FieldInfo modelField)
    {
        if (modelField == null)
        {
            return "";
        }
        else
        {
            object modelObj = Activator.CreateInstance(modelField.FieldType);
            Type type = typeof(TSFrame.MVVM.BindableProperty<>);
            Type interfaceType = type.GetInterfaces()[0];
            PropertyInfo nameProp = interfaceType.GetProperty("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Type bindType = typeof(TSFrame.BindingAttribute);
            Type elementType = typeof(TSFrame.UI.IElement);
            StringBuilder stringBuilder = new StringBuilder();
            FieldInfo[] panelFieldInfos = panelType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            FieldInfo[] injectFieldInfos = modelField.FieldType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var item in injectFieldInfos)
            {
                if (interfaceType.IsAssignableFrom(item.FieldType))
                {
                    object[] objs = item.GetCustomAttributes(bindType, false);
                    if (objs.Length == 1)
                    {
                        object tempName = nameProp.GetValue(item.GetValue(modelObj));
                        string interfaceName = tempName == null ? null : tempName.ToString();

                        BindingAttribute bindingAttribute = objs[0] as BindingAttribute;
                        string injectFieldName = item.Name;
                        if (interfaceName != injectFieldName)
                        {
                            ErrorList.Add($"{modelField.FieldType.Name}数据类中字段：{injectFieldName}和配置名：{interfaceName}不一致");
                        }
                        foreach (var panelFieldInfo in panelFieldInfos)
                        {
                            if (elementType.IsAssignableFrom(panelFieldInfo.FieldType))
                            {
                                string element = panelFieldInfo.Name;
                                int index = element.IndexOf('_');
                                index++;
                                string tempElementName = element.Substring(index, element.Length - index);
                                if (tempElementName.ToLower() == injectFieldName.ToLower())
                                {
                                    if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OneWay) > 0)
                                    {
                                        stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{modelField.Name}.{injectFieldName}, this.{element}, BindingMode.OneWay);");
                                        stringBuilder.AppendLine();
                                    }
                                    if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OneWayToSource) > 0)
                                    {
                                        stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{modelField.Name}.{injectFieldName}, this.{element}, BindingMode.OneWayToSource);");
                                        stringBuilder.AppendLine();
                                    }
                                    if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.OnTime) > 0)
                                    {
                                        stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{modelField.Name}.{injectFieldName}, this.{element}, BindingMode.OnTime);");
                                        stringBuilder.AppendLine();
                                    }
                                    if ((bindingAttribute.Mode & TSFrame.MVVM.BindingMode.TwoWay) > 0)
                                    {
                                        stringBuilder.AppendLine($"        this.BindingContext.Bind(this.{modelField.Name}.{injectFieldName}, this.{element}, BindingMode.TwoWay);");
                                        stringBuilder.AppendLine();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
    /// <summary>
    /// 获取当前panel的viewModel
    /// </summary>
    /// <param name="panelType"></param>
    /// <returns></returns>
    internal static string GetInjectModelField(Type panelType, ref FieldInfo viewModel)
    {
        if (panelType == null)
        {
            return "";
        }
        else
        {
            viewModel = null;
            StringBuilder sb = new StringBuilder();
            FieldInfo[] fieldInfos = panelType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Type attrType = typeof(InjectAttribute);
            foreach (var fieldInfo in fieldInfos)
            {
                object[] obj = fieldInfo.GetCustomAttributes(attrType, false);
                if (obj.Length > 0)
                {
                    if ((obj[0] as InjectAttribute) != null)
                    {
                        sb.AppendLine($"		this.{fieldInfo.Name} = new {fieldInfo.FieldType}();");
                        if (viewModel == null)
                        {
                            viewModel = fieldInfo;
                        }
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// 获取变量名代码
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="iniTool"></param>
    /// <returns></returns>
    internal static string GetVarCode(List<TranDto> trans, IniTool iniTool)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in trans)
        {
            string typeName = GetExportType(item.Tran.name, iniTool);
            string mvvmTypeName = GetMVVMExportType(typeName);

            if (string.IsNullOrWhiteSpace(mvvmTypeName))
            {
                sb.AppendLine($"    private {typeName} {item.Tran.name} = null;");
            }
            else
            {
                sb.AppendLine($"    private {typeName} raw_{item.Tran.name} = null;");
                sb.AppendLine($"    private {mvvmTypeName} {item.Tran.name} = null;");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
    /// <summary>
    /// 生成组件代码
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    internal static string GetComponentCode(List<TranDto> trans, IniTool iniTool)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var item in trans)
        {
            string typeName = UIEditorUtils.GetExportType(item.Tran.name, iniTool);
            string mvvmTypeName = UIEditorUtils.GetMVVMExportType(typeName);
            if (typeName == typeof(Transform).Name)
            {
                sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\");");
            }
            else if (typeName == typeof(GameObject).Name)
            {
                sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").gameObject;");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(mvvmTypeName))
                {
                    sb.AppendLine($"        this.{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").GetComponent<{typeName}>();");
                }
                else
                {
                    sb.AppendLine($"        this.raw_{item.Tran.name} = this.transform.Find(\"{item.ParentPath}{item.Tran.name}\").GetComponent<{typeName}>();");
                    sb.AppendLine();
                    sb.AppendLine($"        this.BindingElement(this.raw_{item.Tran.name}, out this.{item.Tran.name});");
                }
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
    /// <summary>
    /// 获取所有需要生成的Tran
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="parentPath"></param>
    /// <param name="trans"></param>
    internal static void GetTrans(Transform transform, string parentPath, List<TranDto> trans)
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tran = transform.GetChild(i);
                string path = parentPath;
                if (transform.parent != null)
                {
                    path += (transform.name + "/");
                }
                GetTrans(tran, path, trans);
            }

        }
        if (transform.tag == UIEditorUtils.TAG_NAME)
        {
            TranDto tranDto = new TranDto();
            tranDto.Tran = transform;
            tranDto.ParentPath = parentPath;
            trans.Add(tranDto);
        }
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


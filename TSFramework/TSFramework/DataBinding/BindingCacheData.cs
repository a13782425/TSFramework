using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    /// <summary>
    /// 数据绑定缓存数据
    /// </summary>
    internal static class BindingCacheData
    {
        private static Dictionary<string, List<FieldInfo>> _fieldInfoDic;
        private static Type _bindableType = typeof(IBindableProperty);
        static BindingCacheData()
        {
            _fieldInfoDic = new Dictionary<string, List<FieldInfo>>();
        }

        internal static List<FieldInfo> GetFieldInfos(Type type)
        {
            string typeName = type.FullName;
            if (_fieldInfoDic.ContainsKey(typeName))
            {
                return _fieldInfoDic[typeName];
            }
            else
            {
                List<FieldInfo> tempList = new List<FieldInfo>();
                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var item in fieldInfos)
                {
                    if (_bindableType.IsAssignableFrom(item.FieldType))
                    {
                        tempList.Add(item);
                    }
                }
                _fieldInfoDic.Add(typeName, tempList);
                return tempList;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace TSFrame.UI
{
    /// <summary>
    /// 下拉菜单
    /// </summary>
    public sealed class UIDropdown : UIElement<Dropdown>, IBindingElement
    {
        internal UIDropdown(UIView uIView, Dropdown control) : base(uIView, control)
        {
            Element.onValueChanged.AddListener(tempValueChange);
        }


        /// <summary>
        /// Scrollbar的值改变
        /// </summary>
        public event OnIntValueChanged onValueChanged = null;
        public int value
        {
            get { return Element.value; }
            set
            {
                if (value != Element.value)
                {
                    Element.value = value;
                }
            }
        }
        public event ValueChangedEvent ValueChanged;

        public void SetValue(object obj)
        {
            if (obj == null)
            {
                GameApp.Instance.LogError("需要设置的值为Null===" + Element.name);
                value = 0;
                return;
            }
            switch (obj)
            {
                case int i:
                    value = i;
                    break;
                case string str:
                    if (int.TryParse(str, out int tempi))
                        value = tempi;
                    break;
                case short s:
                    value = s;
                    break;
                case float f:
                    value = (int)f;
                    break;
                case double d:
                    value = (int)d;
                    break;
                case long l:
                    value = (int)l;
                    break;
                case uint ui:
                    value = (int)ui;
                    break;
                case ulong ul:
                    value = (int)ul;
                    break;
                default:
                    GameApp.Instance.LogError("设置类型不支持：" + obj.GetType().Name);
                    break;
            }
        }


        private void tempValueChange(int val)
        {
            onValueChanged?.Invoke(val);
            ValueChanged?.Invoke(val);
        }
        protected override void OnDestroy()
        {
            Element.onValueChanged.RemoveAllListeners();
            onValueChanged = null;
        }
    }
}

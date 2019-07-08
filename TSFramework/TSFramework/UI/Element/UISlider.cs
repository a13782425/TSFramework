using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Slider;

namespace TSFrame.UI
{
    public sealed class UISlider : UIElement<Slider>, IBindingElement
    {
        internal UISlider(UIView uIView, Slider control) : base(uIView, control)
        {
            //_supportType = new Type[]
            //{
            //    typeof(float),
            //    typeof(double),
            //    typeof(int),
            //    typeof(long),
            //    typeof(uint),
            //    typeof(ulong),
            //    typeof(string)
            //};

            Element.onValueChanged.AddListener(tempValueChange);
        }

        /// <summary>
        /// 绑定的值改变
        /// </summary>
        public event ValueChangedEvent ValueChanged;

        /// <summary>
        /// Slider
        /// </summary>
        public event OnFloatValueChanged onValueChanged = null;
        public float value
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

        public void SetValue(object obj)
        {
            if (obj == null)
            {
                Debug.LogError("需要设置的值为Null===" + Element.name);
                value = 0;
                return;
            }
            switch (obj)
            {
                case float f:
                    value = f;
                    break;
                case double d:
                    value = (float)d;
                    break;
                case string str:
                    if (float.TryParse(str, out float tempf))
                        value = tempf;
                    break;
                case int i:
                    value = Mathf.Clamp(i, 0f, 1f);
                    break;
                case long l:
                    value = Mathf.Clamp(l, 0f, 1f);
                    break;
                case uint ui:
                    value = Mathf.Clamp(ui, 0f, 1f);
                    break;
                case ulong ul:
                    value = Mathf.Clamp(ul, 0f, 1f);
                    break;
                default:
                    GameApp.Instance.LogError("设置类型不支持：" + obj.GetType().Name);
                    break;
            }
        }

        private void tempValueChange(float val)
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

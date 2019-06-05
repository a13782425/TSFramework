using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine.UI;
using UnityEngine;
using TSFrame.UI.Base;

namespace TSFrame.UI
{
    /// <summary>
    /// 框架内输入框
    /// </summary>
    public sealed class UIInputField : UIControl<InputField>
    {
        #region 构造

        private static Type _stringType = typeof(string);
        public UIInputField(InputField control) : base(control)
        {

        }

        #endregion

        /// <summary>
        /// 内容
        /// </summary>
        public string text
        {
            get { return Control?.text; }
            set { Control.text = value; }
        }

        public override void OneWayToSource<TField>(BindableProperty<TField> property)
        {
            if (_stringType != typeof(TField))
            {
                Debug.LogError("InputField 只能反向绑定String的ViewModel的变量");
                return;
            }
            OneWayToSource(property as BindableProperty<string>);
        }

        public override void OneWay<TField>(BindableProperty<TField> property)
        {
            property.OnValueChanged += (a, b) =>
            {
                string str = property.Value.ToString();
                if (this.text != str)
                {
                    this.text = property.Value.ToString();
                }
            };
        }

        public override void TwoWay<TField>(BindableProperty<TField> property)
        {
            OneWayToSource(property);
            OneWay(property);
        }


        private void OneWayToSource(BindableProperty<string> property)
        {
            this.Control.onValueChanged.AddListener((a) => { property.Value = a; });
        }




        //public override void OneWayToSource(Action<object> action)
        //{
        //    _control.onValueChanged.AddListener((str) =>
        //    {
        //        action?.Invoke(str);
        //    });
        //}


        //public void OneWay(BindableProperty<string> bindableProperty)
        //{
        //    bindableProperty.OnValueChanged = (a, b) => { _control.text = b; };
        //}


        public override void Freed()
        {
            this.Control.onValueChanged.RemoveAllListeners();
        }
    }
}


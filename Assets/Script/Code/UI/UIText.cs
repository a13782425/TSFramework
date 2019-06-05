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
    public sealed class UIText : UIControl<Text>
    {
        public UIText(Text control) : base(control)
        {
        }
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
            throw new Exception($"Text不能反向绑定ViewModel参数");
        }

        public override void OneWay<TField>(BindableProperty<TField> property)
        {
            property.OnValueChanged += (a, b) => { this.text = property.Value.ToString(); };
        }
    }
}

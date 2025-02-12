﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine.UI;
using UnityEngine;

namespace TSFrame.UI
{
    public sealed class UIText : UIElement<Text>,IBindingElement
    {
        public UIText(Text control) : base(control)
        {
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string text
        {
            get { return Element?.text; }
            set { Element.text = value; }
        }

        public event ValueChangedEvent ValueChanged;

        public override void Binding<TField>(BindableProperty<TField> property)
        {
            Debug.LogError("Text组件不支持反向绑定ViewModel");
        }

        public override void SetValue(object value)
        {
            text = value.ToString();
        }
    }
}

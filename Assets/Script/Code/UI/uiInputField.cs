﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.UI.InputField;

namespace TSFrame.UI
{
    /// <summary>
    /// 框架内输入框
    /// </summary>
    public sealed class UIInputField : UIElement<InputField>, IBindingElement
    {
        public delegate void OnValueChanged(string value);

        #region 构造
        public UIInputField(InputField control) : base(control)
        {
            _supportType = new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(float),
                typeof(double),
                typeof(uint)
            };
            this.Element.onValueChanged.AddListener(tempValueChange);
        }

        #endregion

        #region 组件变量

        /// <summary>
        /// 内容
        /// </summary>
        public string text
        {
            get { return Element?.text; }
            set { Element.text = value; }
        }

        public event ValueChangedEvent ValueChanged;

        private OnValueChanged _onValueChanged;
        public OnValueChanged onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }
        #endregion

        public override void SetValue(object value)
        {
            string str = value.ToString();
            if (str != text)
            {
                text = str;
            }
        }

        private void tempValueChange(string value)
        {
            onValueChanged?.Invoke(value);
            ValueChanged(value);
        }

        public override void Freed()
        {
            this.Element.onValueChanged.RemoveAllListeners();
        }
    }
}


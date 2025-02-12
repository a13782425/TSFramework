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
        //public delegate void OnValueChanged(string value);

        #region 构造
        internal UIInputField(UIView uIView, InputField control) : base(uIView, control)
        {
            //_supportType = new Type[]
            //{
            //    typeof(string),
            //    typeof(int),
            //    typeof(float),
            //    typeof(double),
            //    typeof(uint)
            //};
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

        /// <summary>
        /// 绑定数据变化事件
        /// </summary>
        public event ValueChangedEvent ValueChanged;

        /// <summary>
        /// 输入框文本变化
        /// </summary>
        public event OnStringValueChanged onValueChanged = null;
        #endregion

        public void SetValue(object value)
        {
            string str = value == null ? "" : value.ToString();
            if (str != text)
            {
                text = str;
            }
        }

        private void tempValueChange(string value)
        {
            onValueChanged?.Invoke(value);
            ValueChanged?.Invoke(value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.Element.onValueChanged.RemoveAllListeners();
            onValueChanged = null;
        }
    }
}


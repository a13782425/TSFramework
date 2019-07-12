﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public sealed class UIImage : UIElement<Image>, IBindingElement
    {
        internal UIImage(UIView uIView, Image control) : base(uIView, control)
        {
        }

        public Sprite sprite { get { return Element.sprite; } set { Element.sprite = value; } }

        public float fillAmount { get { return Element.fillAmount; } set { Element.fillAmount = value; } }

        public event ValueChangedEvent ValueChanged;

        public void SetValue(object value)
        {
            if (value == null)
            {
                GameApp.Instance.LogError("需要设置的精灵为Null===" + Element.name);
                sprite = null;
                return;
            }
            switch (value)
            {
                case Sprite sp:
                    sprite = sp;
                    break;
                case string str:
                    sprite = GameApp.Instance.ResourcesLoader.Load<Sprite>(str);
                    break;
                case float f:
                    fillAmount = f;
                    break;
                case double d:
                    fillAmount = (float)d;
                    break;
                default:
                    GameApp.Instance.LogError("设置类型不支持：" + value.GetType().Name);
                    sprite = null;
                    break;
            }
        }


    }


}
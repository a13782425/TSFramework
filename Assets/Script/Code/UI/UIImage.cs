using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public sealed class UIImage : UIElement<Image>,IBindingElement
    {
        public UIImage(Image control) : base(control)
        {
            _supportType = new Type[]
            {
                typeof(string),
                typeof(Sprite)
            };
        }

        public Sprite sprite { get { return Element.sprite; } set { Element.sprite = value; } }

        public float fillAmount { get { return Element.fillAmount; } set { Element.fillAmount = value; } }

        public event ValueChangedEvent ValueChanged;

        public override void SetValue(object value)
        {
            if (value == null)
            {
                Debug.LogError("需要设置的精灵为Null===" + Element.name);
                sprite = null;
                return;
            }
            switch (value)
            {
                case Sprite sp when value is Sprite:
                    sprite = sp;
                    break;
                case string str when value is string:
                    //todo Resources.Load
                    break;
                default:
                    Debug.LogError("设置类型不支持：" + value.GetType().Name);
                    sprite = null;
                    break;
            }
        }
        public override void Binding<TField>(BindableProperty<TField> property)
        {
            Debug.LogError("Image组件不支持反向绑定ViewModel");
        }


    }


}
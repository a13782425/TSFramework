using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public sealed class UIToggle : UIElement<Toggle>, IBindingElement
    {
        internal UIToggle(UIView uIView, Toggle control) : base(uIView, control)
        {
            //_supportType = new Type[]
            //{
            //    typeof(bool)
            //};
            Element.onValueChanged.AddListener(tempValueChange);
        }

        public bool isOn { get { return Element.isOn; } set { if (Element.isOn != value) Element.isOn = value; } }
        public ToggleGroup group { get { return Element.group; } set { Element.group = value; } }


        public event ValueChangedEvent ValueChanged;
        private OnBoolValueChanged _onValueChanged;

        public OnBoolValueChanged onValueChanged
        {
            get { return _onValueChanged; }
            set { _onValueChanged = value; }
        }

        public void SetValue(object value)
        {
            if (value != null)
            {
                if (value is bool)
                {
                    isOn = (bool)value;
                }
            }
        }

        private void tempValueChange(bool value)
        {
            onValueChanged?.Invoke(value);
            ValueChanged?.Invoke(value);
        }

        protected override void OnDestroy()
        {
            this.Element.onValueChanged.RemoveAllListeners();
        }
    }
}

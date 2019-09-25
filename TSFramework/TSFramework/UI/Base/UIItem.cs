using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;

namespace TSFrame.UI
{
    public abstract class UIItem : UIView
    {
        private UIView _belongView = null;
        public UIView BelongView { get => _belongView; internal set => _belongView = value; }
        protected UIItem() : base()
        {
        }

        protected override void OnDestroy()
        {
            BelongView?.RemoveElement(this);
            base.OnDestroy();
        }
    }
}

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
        private UIPanel _belongPanel = null;
        public UIPanel BelongPanel { get => _belongPanel; internal set => _belongPanel = value; }
        protected UIItem() : base()
        {
        }

        protected override void OnDestroy()
        {
            BelongPanel?.RemoveElement(this);
            base.OnDestroy();
        }
    }
}

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
        public UIItem(GameObject obj) : base(obj)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine;

namespace TSFrame.UI
{
    public abstract class UIPanel : UIView
    {
        public virtual UILayerEnum LayerEnum => UILayerEnum.Normal;

        protected UIPanel() : base()
        {

        }
        public T AddSubItem<T>() where T : UIItem, new()
        {
            return AddSubItem<T>(null);
        }
        public T AddSubItem<T>(Transform parent) where T : UIItem, new()
        {
            T t = UIView.CreateView<T>(parent);

            return t;
        }
    }
}

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
        /// <summary>
        /// UI层级
        /// </summary>
        public virtual UILayerEnum LayerEnum => UILayerEnum.Normal;

        private bool _useItemPool = false;

        protected UIPanel() : base()
        {

        }
        /// <summary>
        /// 添加Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddSubItem<T>() where T : UIItem, new()
        {
            return AddSubItem<T>(null);
        }

        /// <summary>
        /// 添加Item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">默认父亲</param>
        /// <returns></returns>
        public T AddSubItem<T>(Transform parent) where T : UIItem, new()
        {
            T t = UIView.CreateView<T>(parent);
            t.BelongPanel = this;
            BindingElement(t);
            return t;
        }

        public void RemoveItem(UIItem item)
        {
            item.Close();
        }
    }
}

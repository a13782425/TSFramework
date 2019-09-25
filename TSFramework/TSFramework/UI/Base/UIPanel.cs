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
        /// 设置层级
        /// </summary>
        /// <param name="uILayer"></param>
        [Obsolete("此方法可能导致意料之外的情况，请设置 LayerEnum 的值")]
        public void SetLayer(UILayerEnum uILayer)
        {
            Transform tran = GameApp.Instance.UILoader.GetParent(uILayer);

            this.SetParent(tran);
        }

        /// <summary>
        /// 删除一个Item
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(UIItem item)
        {
            item.Close();
        }
    }
}

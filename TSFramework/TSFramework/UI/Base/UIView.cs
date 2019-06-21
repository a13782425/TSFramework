using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;

namespace TSFrame.UI
{

    public abstract class UIView : UIElement
    {
        /// <summary>
        /// UI路径
        /// </summary>
        public abstract string UIPath { get; }
        protected List<UIElement> uiControlList { get => _uiControlList; }

        private List<UIElement> _uiControlList;

        private Binding _bindingContext = null;

        public Binding BindingContext { get => _bindingContext; }

        public UIView()
        {
            _bindingContext = new Binding();
        }

        /// <summary>
        /// UI创建
        /// </summary>
        protected virtual void OnCreate()
        {

        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected virtual void Binding()
        {

        }

        #region Static

        /// <summary>
        /// 创建一个View
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        public static T1 CreateView<T1>() where T1 : UIView, new()
        {
            T1 t = new T1();
            t._uiControlList = new List<UIElement>();
            t.OnCreate();
            t.Binding();
            return t;
        }


        #endregion
    }

    //public abstract class UIView<T> : UIView where T : BindingModel, new()
    //{

    //    //private Binding<T> _bindingContext = null;

    //    //public Binding<T> BindingContext { get => _bindingContext; }

    //    ////private T _bindingContext = null;

    //    ////public T BindingContext { get => _bindingContext; protected set => _bindingContext = value; }


    //    //public UIView()
    //    //{
    //    //    _bindingContext = new Binding<T>(this);
    //    //}

    //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.MVVM;
using UnityEngine.EventSystems;

namespace TSFrame.UI.Base
{
    public abstract class UIControl<T> where T : UIBehaviour
    {
        private T _control = null;
        public T Control => _control;

        public UIControl(T control)
        {
            if (control == null)
            {
                throw new Exception($"空间初始化为空：{typeof(T).GetType().FullName}");
            }
            _control = control;
        }

        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="property">UI 需要绑定的变量</param>
        /// <param name="bindingMode">绑定类型，默认OneWay</param>
        public void Binding<TField>(BindableProperty<TField> property, BindingMode bindingMode = BindingMode.OneWay)
        {
            Binding(property, null, null, bindingMode);
        }
        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="property">UI 需要绑定的变量</param>
        /// <param name="vmAction">变量改变时的回调方法</param>
        /// <param name="bindingMode">绑定类型，默认OneWay</param>
        public void Binding<TField>(BindableProperty<TField> property, Action<TField, TField> vmAction, BindingMode bindingMode = BindingMode.OneWay)
        {
            Binding(property, vmAction, null, bindingMode);
        }
        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="property">UI 需要绑定的变量</param>
        /// <param name="uiAction">UI改变时的回调方法</param>
        public void Binding<TField>(BindableProperty<TField> property, Action<object> uiAction)
        {
            OneWayToSource(property, uiAction);
        }
        /// <summary>
        /// 绑定方法
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="property">UI 需要绑定的变量</param>
        /// <param name="vmAction">变量改变时的回调方法</param>
        /// <param name="uiAction">UI改变时的回调方法</param>
        /// <param name="bindingMode">绑定类型</param>
        public void Binding<TField>(BindableProperty<TField> property, Action<TField, TField> vmAction, Action<object> uiAction, BindingMode bindingMode)
        {

        }
        public virtual void Freed()
        {

        }

        /// <summary>
        /// 当ViewModel值发生改变的时候修改组件
        /// </summary>
        protected virtual void OneWay<TField>(BindableProperty<TField> property, Action<TField, TField> vmAction)
        {

        }
        /// <summary>
        /// 当组件的值发生改变的时候修改ViewModel的值
        /// </summary>
        protected virtual void OneWayToSource<TField>(BindableProperty<TField> property, Action<object> uiAction)
        {

        }
        /// <summary>
        /// 组件和ViewModel值互相绑定（慎用可能会出现递归引用）
        /// </summary>
        protected virtual void TwoWay<TField>(BindableProperty<TField> property, Action<TField, TField> vmAction, Action<object> uiAction)
        {

        }
        /// <summary>
        /// 在绑定ViewModel的时候执行一次
        /// </summary>
        protected virtual void OneTime<TField>(BindableProperty<TField> property, Action<TField, TField> vmAction)
        {

        }



    }
}

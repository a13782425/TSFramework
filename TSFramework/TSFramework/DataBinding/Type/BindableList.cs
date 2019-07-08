using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TSFrame.MVVM
{
    public sealed class BindableList<T> : IEnumerable<T>
    {
        public delegate void OnValueChangeDelegate(List<T> list);

        #region Variable

        private List<T> _list;

        public int Count => _list.Count;

        private OnValueChangeDelegate onValueChange = null;

        #endregion

        #region ctor

        public BindableList()
        {
            _list = new List<T>();
        }
        public BindableList(IEnumerable<T> array)
        {
            _list = new List<T>(array);
        }

        #endregion

        #region Method

        public BindableList<T> Add(T item, bool publish = true)
        {
            this._list.Add(item);
            if (publish)
            {
                Publish();
            }
            return this;
        }

        public BindableList<T> AddRange(IEnumerable<T> collection, bool publish = true)
        {
            this._list.AddRange(collection);

            return Publish(publish);
        }

        public BindableList<T> Remove(T item, bool publish = true)
        {
            this._list.Remove(item);
            return Publish(publish);
        }

        public BindableList<T> RemoveAll(Predicate<T> match, bool publish = true)
        {
            this._list.RemoveAll(match);
            return Publish(publish);
        }

        public BindableList<T> Clear(bool publish = true)
        {
            this._list.Clear();
            return Publish(publish);
        }
        public BindableList<T> RemoveAt(int index, bool publish = true)
        {
            this._list.RemoveAt(index);
            return Publish(publish);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        public BindableList<T> Subscribe(OnValueChangeDelegate valueChange)
        {
            onValueChange += valueChange;
            return this;
        }

        public BindableList<T> Unsubscribe(OnValueChangeDelegate valueChange)
        {
            onValueChange -= valueChange;
            return this;
        }
        /// <summary>
        /// 发布
        /// </summary>
        public BindableList<T> Publish()
        {
            List<T> temp = new List<T>(_list);
            onValueChange?.Invoke(temp);

            return this;
        }
        #endregion

        #region private

        private BindableList<T> Publish(bool publish)
        {
            if (publish)
            {
                Publish();
            }
            return this;
        }
        #endregion

        #region Enumerable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}

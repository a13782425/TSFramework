using TSFrame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TSFrame
{
    /// <summary>
    /// 对象池
    /// </summary>
    public sealed class ObjectPool
    {
        #region 单例对象
        private static ObjectPool _instance = null;
        /// <summary>
        /// 单例对象
        /// </summary>
        public static ObjectPool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ObjectPool();
                return _instance;
            }
        }
        #endregion

        private GameObject _gameObject;
        public GameObject gameObject => _gameObject;

        public Transform transform => gameObject.transform;
        private Dictionary<string, IPoolData> _poolDic = null;
        private ObjectPool()
        {
            _gameObject = new GameObject("ObjectPool");
            gameObject.transform.SetParent(GameApp.Instance.transform);
            gameObject.SetActive(false);
            gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            transform.position = Vector3.zero;
            _poolDic = new Dictionary<string, ObjectPool.IPoolData>();
        }
        /// <summary>
        /// 创建对象池(使用类型名)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="createNew">创建对象方法</param>
        /// <param name="recoveAction">回收对象回调(T,对象池Tran)</param>
        /// <param name="destroyAciton">销毁对象调用方法</param>
        /// <param name="initCount">初始化数量</param>
        /// <returns></returns>
        public PoolData<T> CreatePool<T>(Func<T> createNew, Action<T, Transform> recoveAction, Action<T> destroyAciton, int initCount = 4)
        {
            return CreatePool(typeof(T).Name, createNew, recoveAction, destroyAciton, initCount);
        }
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName">对象池名</param>
        /// <param name="createNew">创建对象方法</param>
        /// <param name="recoveAction">回收对象回调(T,对象池Tran)</param>
        /// <param name="destroyAciton">销毁对象调用方法</param>
        /// <param name="initCount">初始化数量</param>
        /// <returns></returns>
        public PoolData<T> CreatePool<T>(string poolName, Func<T> createNew, Action<T, Transform> recoveAction, Action<T> destroyAciton, int initCount = 4)
        {
            if (createNew == null || recoveAction == null || destroyAciton == null)
            {
                throw new Exception("某个委托方法为空的方法！");
            }
            if (_poolDic.ContainsKey(poolName))
            {
                return _poolDic[poolName] as PoolData<T>;
            }
            PoolData<T> pool = new PoolData<T>(poolName, createNew, recoveAction, destroyAciton, initCount);
            _poolDic.Add(poolName, pool);
            return pool;
        }

        /// <summary>
        /// 删除对象池(使用类型名)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ObjectPool DestroyPool<T>() where T : UIElement
        {
            return DestroyPool(typeof(T).Name);
        }
        /// <summary>
        /// 删除对象池
        /// </summary>
        /// <param name="poolName">对象池名</param>
        /// <returns></returns>
        public ObjectPool DestroyPool(string poolName)
        {
            if (_poolDic.ContainsKey(poolName))
            {
                IPoolData pool = _poolDic[poolName];
                _poolDic.Remove(poolName);
                pool.Destroy();
            }

            return this;
        }
        /// <summary>
        /// 获取一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObject<T>()
        {
            return GetObject<T>(typeof(T).Name);
        }
        /// <summary>
        /// 获取一个对象(使用类型名)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName">对象池名字</param>
        /// <returns></returns>
        public T GetObject<T>(string poolName)
        {
            if (_poolDic.ContainsKey(poolName))
            {
                return (T)_poolDic[poolName].GetNew();
            }
            GameApp.Instance.LogError($"对象池:{poolName} 不存在");
            return default(T);
        }
        /// <summary>
        /// 回收一个对象(使用类型名)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象</param>
        public void RecoveObject<T>(T obj)
        {
            RecoveObject<T>(typeof(T).Name, obj);
        }
        /// <summary>
        /// 回收一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName">对象池名字</param>
        /// <param name="obj">对象</param>
        public void RecoveObject<T>(string poolName, T obj)
        {
            if (_poolDic.ContainsKey(poolName))
            {
                _poolDic[poolName].Recove(obj);
            }
            GameApp.Instance.LogError($"对象池:{poolName} 不存在");
        }
        internal interface IPoolData
        {
            object GetNew();
            void Recove(object obj);
            void Destroy();
        }
        /// <summary>
        /// 对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class PoolData<T> : IPoolData
        {
            /// <summary>
            /// 对象池名字
            /// </summary>
            public string Name { get; private set; }
            private GameObject _gameObject;
            public GameObject gameObject => _gameObject;
            public Transform transform => gameObject.transform;
            /// <summary>
            /// 创建方法
            /// </summary>
            private Func<T> _createNew = null;
            /// <summary>
            /// 回收方法
            /// </summary>
            private Action<T, Transform> _recoveAction = null;
            /// <summary>
            /// 删除方法
            /// </summary>
            private Action<T> _destroyAciton = null;
            /// <summary>
            /// 待使用队列
            /// </summary>
            private Queue<T> _waitUseQueue = null;
            /// <summary>
            /// 已使用队列
            /// </summary>
            private HashSet<T> _useSet = null;
            internal PoolData(string name, Func<T> createNew, Action<T, Transform> recoveAction, Action<T> destroyAciton, int initCount)
            {
                Name = name;
                UnityEngine.Object
                _gameObject = new GameObject(name);
                gameObject.SetActive(false);
                transform.SetParent(Instance.transform);
                transform.position = Vector3.zero;
                _waitUseQueue = new Queue<T>();
                _useSet = new HashSet<T>();
                _createNew = createNew;
                _recoveAction = recoveAction;
                _destroyAciton = destroyAciton;
                for (int i = 0; i < initCount; i++)
                {
                    _waitUseQueue.Enqueue(_createNew.Invoke());
                }
            }

            /// <summary>
            /// 回收对象
            /// </summary>
            /// <param name="obj">对象</param>
            public void Recove(T obj)
            {
                _useSet.Remove(obj);
                _waitUseQueue.Enqueue(obj);
                _recoveAction?.Invoke(obj, transform);
            }
            /// <summary>
            /// 获取一个新对象
            /// </summary>
            /// <returns></returns>
            public T GetNew()
            {
                T t = default(T);
                if (_waitUseQueue.Count > 0)
                {
                    t = _waitUseQueue.Dequeue();
                }
                else
                {
                    t = _createNew.Invoke();
                }
                _useSet.Add(t);
                return t;
            }

            object IPoolData.GetNew()
            {
                return GetNew();
            }

            void IPoolData.Recove(object obj)
            {
                if (obj is T t)
                {
                    Recove(t);
                }
            }

            void IPoolData.Destroy()
            {
                while (_useSet.Count > 0)
                {
                    T t = _useSet.First();

                    Recove(t);
                }
                while (_waitUseQueue.Count > 0)
                {
                    T t = _waitUseQueue.Dequeue();
                    _destroyAciton.Invoke(t);
                }
                GameObject.Destroy(this.gameObject);
            }

        }
    }
}

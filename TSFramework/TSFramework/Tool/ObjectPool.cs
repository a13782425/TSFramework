using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        private ObjectPool()
        {
            _gameObject = new GameObject("ObjectPool");
            _gameObject.transform.SetParent(GameApp.Instance.transform);
            _gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            _gameObject.transform.position = Vector3.zero;
        }


        public ObjectPool CreatePool(string poolName)
        {
            return this;
        }
    }
}

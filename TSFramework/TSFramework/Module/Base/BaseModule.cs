using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame;


namespace TSFrame.Module
{
    public abstract class BaseModule<T> : IModule where T : class, IModule, new()
    {
        private static T _instance = null;

        public static T Instance => _instance;

        private UnityEngine.GameObject _gameObject;
        public UnityEngine.GameObject gameObject => _gameObject;

        public UnityEngine.Transform transform => _gameObject?.transform;


        public BaseModule()
        {
            _gameObject = new UnityEngine.GameObject(this.GetType().Name);
            _gameObject.transform.SetParent(GameApp.Instance.transform);
            _instance = this as T;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime">帧时间</param>
        public virtual void Update(float deltaTime)
        {

        }
        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Freed()
        {

        }
    }
}

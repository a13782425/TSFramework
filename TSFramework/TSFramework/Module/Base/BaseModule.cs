using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame;
using UnityEngine;

namespace TSFrame.Module
{
    public abstract class BaseModule<T> : IModule where T : class, IModule, new()
    {
        private static T _instance = null;

        public static T Instance => _instance;

        private UnityEngine.GameObject _gameObject;
        public UnityEngine.GameObject gameObject => _gameObject;

        public UnityEngine.Transform transform => _gameObject?.transform;

        private Dictionary<int, Coroutine> _coroutineDic;


        public BaseModule()
        {
            _gameObject = new UnityEngine.GameObject(this.GetType().Name);
            _gameObject.transform.SetParent(GameApp.Instance.transform);
            _coroutineDic = new Dictionary<int, Coroutine>();
            _instance = this as T;
            if (_instance == null)
            {
                GameApp.Instance.LogError($"ModuleType:{this.GetType().Name},=== T Type:{typeof(T).Name}");
            }
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

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            Coroutine coroutine = GameApp.Instance.StartCoroutine(routine);
            _coroutineDic.Add(coroutine.GetHashCode(), coroutine);
            return coroutine;
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopCoroutine(Coroutine coroutine)
        {
            GameApp.Instance.StopCoroutine(coroutine);
            int hashCode = coroutine.GetHashCode();
            if (_coroutineDic.ContainsKey(hashCode))
            {
                _coroutineDic.Remove(hashCode);
            }
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        public void StopAllCoroutines()
        {
            foreach (var item in _coroutineDic)
            {
                GameApp.Instance.StopCoroutine(item.Value);
            }
            _coroutineDic.Clear();
        }
    }
}

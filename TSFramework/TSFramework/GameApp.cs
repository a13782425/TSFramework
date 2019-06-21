using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TSFrame
{
    public sealed class GameApp
    {

        #region ctor
        private static GameApp _instance = null;

        public static GameApp Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameApp();
                return _instance;
            }
        }

        private GameApp()
        {
            gameObject = new GameObject("GameApp");
            transform = gameObject.transform;
            transform.SetParent(null);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _gameMono = gameObject.AddComponent<GameMono>();
            Object.DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region mono

        private GameMono _gameMono = null;

        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }

        /// <summary>
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public GameApp StartCoroutine(IEnumerator routine, out Coroutine coroutine)
        {
            coroutine = _gameMono.StartCoroutine(routine);
            return this;
        }
        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        public GameApp StopCoroutine(Coroutine coroutine)
        {
            _gameMono.StopCoroutine(coroutine);
            return this;
        }
        /// <summary>
        /// 停止所有协程(慎用)
        /// </summary>
        /// <returns></returns>
        public GameApp StopAllCoroutines()
        {
            _gameMono.StopAllCoroutines();
            return this;
        }

        #endregion

        #region Logger

        private ILogger _logger;
        /// <summary>
        /// 日志
        /// </summary>
        private ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new DefaultLogger();
                }
                return _logger;
            }
        }
        public bool LoggerEnable => Logger.IsEnable;
        /// <summary>
        /// 设置日志类
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public GameApp SetLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }
        /// <summary>
        /// 设置是否启用日志
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public GameApp SetLoggerEnable(bool isEnable)
        {
            Logger.IsEnable = isEnable;
            return this;
        }
        public GameApp Log(object message)
        {
            this.Log(message.ToString());
            return this;
        }
        public GameApp Log(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.Log(message);
            return this;
        }
        public GameApp LogWarn(object message)
        {
            this.LogWarn(message.ToString());
            return this;
        }
        public GameApp LogWarn(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.LogWarn(message);
            return this;
        }
        public GameApp LogError(object message)
        {
            this.LogError(message.ToString());
            return this;
        }
        public GameApp LogError(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.LogError(message);
            return this;
        }

        #endregion

        #region Resources

        #endregion

        #region Time

        private float deltaTime;

            
            

        #endregion


        #region Init

        public GameApp Init()
        {

            return this;
        }

        #endregion

       

        #region Class

        private class GameMono : MonoBehaviour
        {
            void Awake()
            {
            }

            void Update()
            {

            }
        }

        #endregion
    }
}

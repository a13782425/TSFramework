﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSFrame.Module;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TSFrame
{
    /// <summary>
    /// 游戏全局总入口
    /// </summary>
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
            _startTime = DateTime.Now;
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
        private bool _isUseGc = false;
        private float _gcTime = 180;
        private float _gcTempTime = 0;
        /// <summary>
        /// 是否使用GC 
        /// </summary>
        /// <param name="isUse"></param>
        /// <returns></returns>
        public GameApp UseGC(bool isUse)
        {
            _isUseGc = isUse;
            _gcTempTime = 0;
            return this;
        }
        /// <summary>
        /// 设置GC时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public GameApp SetGCTime(float seconds)
        {
            _gcTime = seconds;
            return this;
        }
        void OnUpdate()
        {
            if (_moduleDtos != null)
            {
                foreach (var item in _moduleDtos)
                {
                    item.Module.Update(deltaTime);
                }
            }
            if (_isUseGc)
            {
                if (_gcTempTime >_gcTime)
                {
                    System.GC.Collect();
                    _gcTempTime = 0;
                }
                _gcTempTime += deltaTime;
            }
        }
        void Freed()
        {
            foreach (var item in _moduleDtos)
            {
                item.Module.Freed();
            }
            Object.Destroy(gameObject);
        }
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
        /// 启动协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _gameMono.StartCoroutine(routine);
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

        /// <summary>
        /// 是否启用Log
        /// </summary>
        public bool LoggerEnable => Logger.IsEnable;

        /// <summary>
        /// 设置日志类
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public GameApp SetLogger(ILogger logger)
        {
            logger.IsEnable = true;
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

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp Log(object message)
        {
            this.Log(message.ToString());
            return this;
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp Log(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.Log(message);
            return this;
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp LogWarning(object message)
        {
            this.LogWarning(message.ToString());
            return this;
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp LogWarning(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.LogWarning(message);
            return this;
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp LogError(object message)
        {
            this.LogError(message.ToString());
            return this;
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public GameApp LogError(string message)
        {
            if (!this.LoggerEnable)
                return this;
            this.Logger.LogError(message);
            return this;
        }

        #endregion

        #region Resources

        private IResourcesLoader _resourcesLoader = null;

        /// <summary>
        /// 资源加载器
        /// </summary>
        public IResourcesLoader ResourcesLoader
        {
            get
            {
                if (_resourcesLoader == null)
                {
                    _resourcesLoader = DefaultResourcesModule.Instance;
                }
                return _resourcesLoader;
            }
        }
        /// <summary>
        /// 设置框架内部的资源读取模块
        /// 如果不设置则为默认
        /// </summary>
        /// <param name="resourcesLoader"></param>
        /// <returns></returns>
        public GameApp SetResources(IResourcesLoader resourcesLoader)
        {
            if (resourcesLoader != null)
            {
                if (DefaultResourcesModule.Instance != null)
                {
                    RemoveModule(DefaultResourcesModule.Instance);
                    DefaultResourcesModule.Instance.Freed();
                }
            }
            _resourcesLoader = resourcesLoader;
            return this;
        }

        #endregion

        #region Setting

        /// <summary>
        /// 是否是刘海屏
        /// </summary>
        /// <param name="isNotchScreen"></param>
        /// <returns></returns>
        public GameApp SetNotchScreen(bool isNotchScreen)
        {
            GameSetting.IsNotchScreen = isNotchScreen;
            return this;
        }
        /// <summary>
        /// 设置UI安全区域
        /// </summary>
        /// <param name="safeArea"></param>
        /// <returns></returns>
        public GameApp SetSafeArea(Vector2 safeArea)
        {
            GameSetting.SafeArea = safeArea;
            return this;
        }

        /// <summary>
        /// 设置游戏帧数
        /// </summary>
        /// <param name="frameRate">游戏帧率</param>
        /// <returns></returns>
        public GameApp SetFrameRate(int frameRate)
        {
            GameSetting.FrameRate = frameRate;
            return this;
        }

        #endregion

        #region UI


        private IUILoader _uiLoader = null;

        public IUILoader UILoader
        {
            get
            {
                if (_uiLoader == null)
                {
                    _uiLoader = DefaultUIModule.Instance;
                }
                return _uiLoader;
            }
        }
        /// <summary>
        /// 设置框架内部的资源读取模块
        /// 如果不设置则为默认
        /// </summary>
        /// <param name="resourcesLoader"></param>
        /// <returns></returns>
        public GameApp SetUIModule(IUILoader uiloader)
        {
            if (uiloader != null)
            {
                if (DefaultUIModule.Instance != null)
                {
                    RemoveModule(DefaultUIModule.Instance);
                    DefaultUIModule.Instance.Freed();
                }
            }
            _uiLoader = uiloader;
            return this;
        }


        #endregion

        #region Time

        private GameApp.DeltaTimeType _deltaTimeType = GameApp.DeltaTimeType.DeltaTime;

        private ICalDeltaTime _icalDeltaTime = null;

        public float deltaTime => GetDeltaTime();

        private float GetDeltaTime()
        {
            switch (_deltaTimeType)
            {
                case DeltaTimeType.FixedDeltaTime:
                    return Time.fixedDeltaTime;
                case DeltaTimeType.FixedUnscaledDeltaTime:
                    return Time.fixedUnscaledDeltaTime;
                case DeltaTimeType.UnscaledDeltaTime:
                    return Time.unscaledDeltaTime;
                case DeltaTimeType.RealTime:
                    return _gameMono.deltaTime;
                case DeltaTimeType.Customize:
                    return _icalDeltaTime.GetDeltaTime();
                case DeltaTimeType.DeltaTime:
                default:
                    return Time.deltaTime;
            }
        }
        /// <summary>
        /// 设置deltaTime类型
        /// </summary>
        /// <param name="calDeltaTime"></param>
        /// <returns></returns>
        public GameApp SetDeltaTimeType(ICalDeltaTime calDeltaTime)
        {
            if (calDeltaTime == null)
                LogWarning("计算时间的控制器为空");
            else
            {
                _deltaTimeType = DeltaTimeType.Customize;
                _icalDeltaTime = calDeltaTime;
            }
            return this;
        }
        /// <summary>
        /// 设置deltaTime类型
        /// </summary>
        /// <param name="deltaTimeType"></param>
        /// <returns></returns>
        public GameApp SetDeltaTimeType(GameApp.DeltaTimeType deltaTimeType)
        {
            if (deltaTimeType == DeltaTimeType.Customize)
                LogWarning("Customize 类型请用另一个重载");
            else
                _deltaTimeType = deltaTimeType;
            return this;
        }

        private GameApp.TimeType _timeType = GameApp.TimeType.Time;

        private readonly DateTime _startTime = DateTime.Now;

        private ICalTime _icalTime = null;
        /// <summary>
        /// 游戏时间
        /// </summary>
        public float gameTime => GetGameTime();

        private float GetGameTime()
        {
            switch (_timeType)
            {
                case TimeType.UnscaledTime:
                    return Time.unscaledTime;
                case TimeType.Customize:
                    return _icalTime.GetTime();
                case TimeType.RealTime:
                    return (float)((DateTime.Now - _startTime).TotalSeconds);
                default:
                case TimeType.Time:
                    return Time.time;
            }
        }
        public GameApp SetTimeType(ICalTime calTime)
        {
            if (calTime == null)
                LogWarning("计算时间的控制器为空");
            else
            {
                _timeType = TimeType.Customize;
                _icalTime = calTime;
            }
            return this;
        }
        public GameApp SetTimeType(GameApp.TimeType timeType)
        {
            if (timeType == TimeType.Customize)
                LogWarning("Customize 类型请用另一个重载");
            else
                _timeType = timeType;
            return this;
        }

        #endregion

        #region Init

        private static List<ModuleDto> _moduleDtos = null;

        private const string GameAssemblyFullName = "Assembly-CSharp";
        /// <summary>
        /// 初始化框架
        /// </summary>
        /// <returns></returns>
        public GameApp Init()
        {
            _moduleDtos = new List<ModuleDto>();
            InitModule();
            Application.targetFrameRate = GameSetting.FrameRate;
            return this;
        }

        /// <summary>
        /// 执行Manager
        /// </summary>
        private void InitModule()
        {
            Type moduleType = typeof(IModule);
            Type attrType = typeof(PriorityAttribute);
            Type resType = typeof(IResourcesLoader);
            List<Assembly> assemblieList = new List<Assembly>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in assemblies)
            {
                if (item.GetName().Name == GameAssemblyFullName)
                {
                    assemblieList.Add(item);
                    break;
                }
            }
            assemblieList.Add(moduleType.Assembly);
            foreach (var assembly in assemblieList)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type item in types)
                {
                    if (!item.IsAbstract && moduleType.IsAssignableFrom(item))
                    {
                        //执行模块
                        IModule module = Activator.CreateInstance(item) as IModule;
                        int priority = GetPriority(item, attrType);
                        _moduleDtos.Add(new ModuleDto() { Module = module, Priority = priority });
                    }
                }
            }
            _moduleDtos.Sort((a, b) =>
            {
                if (a.Priority > b.Priority)
                {
                    return 1;
                }
                else if (a.Priority < b.Priority)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            foreach (var item in _moduleDtos)
            {
                item.Module.Init();
            }
        }

        private void RemoveModule<T>(BaseModule<T> module) where T : class, IModule, new()
        {
            ModuleDto dto = null;
            foreach (var item in _moduleDtos)
            {
                if (item.Module is T)
                {
                    dto = item;
                    break;
                }
            }
            if (dto != null)
            {
                _moduleDtos.Remove(dto);
            }
        }

        /// <summary>
        /// 获取执行顺序
        /// </summary>
        /// <param name="bootstrap"></param>
        /// <returns></returns>
        private static int GetPriority(Type targetType, Type attrType)
        {
            object[] priorities = targetType.GetCustomAttributes(attrType, false);
            if (priorities == null || priorities.Length == 0)
            {
                return 0;
            }
            PriorityAttribute priorityAttribute = priorities[0] as PriorityAttribute;
            return priorityAttribute.Priority;
        }
        #endregion

        #region Class

        private class GameMono : MonoBehaviour
        {
            private DateTime _dateTime;
            internal float deltaTime = 0;

            void Start()
            {
                _dateTime = DateTime.Now;
            }

            void Update()
            {
                deltaTime = (float)((DateTime.Now - _dateTime).TotalMilliseconds * 0.001);
                _dateTime = DateTime.Now;
                Instance.OnUpdate();
            }
            private void OnDestroy()
            {
                Instance.Freed();
            }
        }
        private class ModuleDto
        {
            public int Priority = 0;
            public IModule Module = null;
        }
        #endregion

        #region Enum

        public enum DeltaTimeType
        {
            /// <summary>
            /// Time.deltaTime
            /// </summary>
            DeltaTime = 0,
            /// <summary>
            /// Time.unscaledDeltaTime
            /// </summary>
            UnscaledDeltaTime,
            /// <summary>
            /// Time.fixedDeltaTime
            /// </summary>
            FixedDeltaTime,
            /// <summary>
            /// Time.fixedUnscaledDeltaTime
            /// </summary>
            FixedUnscaledDeltaTime,
            /// <summary>
            /// 真实的时间
            /// DateTime.Now
            /// </summary>
            RealTime,
            /// <summary>
            /// 定制
            /// </summary>
            Customize
        }

        public enum TimeType
        {
            /// <summary>
            /// Time.time
            /// </summary>
            Time,
            /// <summary>
            /// Time.unscaledTime
            /// </summary>
            UnscaledTime,
            /// <summary>
            /// 真实的时间
            /// DateTime.Now
            /// </summary>
            RealTime,
            /// <summary>
            /// 定制
            /// </summary>
            Customize
        }

        #endregion

        #region Interface

        public interface ICalDeltaTime
        {
            float GetDeltaTime();
        }
        public interface ICalTime
        {
            float GetTime();
        }
        #endregion
    }
}

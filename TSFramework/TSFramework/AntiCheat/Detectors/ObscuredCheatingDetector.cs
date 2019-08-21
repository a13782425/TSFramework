#if ANTI_CHEAT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSFrame
{
    /// <summary>
    /// 内存模糊探测器
    /// </summary>
    public class ObscuredCheatingDetector : AntiCheatDetectorBase
    {
        private static int instancesInScene;

        #region public fields

        /// <summary>
        /// 允许最大的真假差值
        /// </summary>
        public double DoubleEpsilon = 0.0001d;


        /// <summary>
        /// 允许最大的真假差值
        /// </summary>
        public float FloatEpsilon = 0.0001f;

        /// <summary>
        /// 允许最大的真假差值
        /// </summary>
        public float Vector2Epsilon = 0.1f;

        /// <summary>
        /// 允许最大的真假差值
        /// </summary>
        public float Vector3Epsilon = 0.1f;
        /// <summary>
        /// 允许最大的真假差值
        /// </summary>
        public float QuaternionEpsilon = 0.1f;
        #endregion

        #region public static methods
        /// <summary>
        /// Creates new instance of the detector at scene if it doesn't exists. Make sure to call NOT from Awake phase.
        /// </summary>
        /// <returns>New or existing instance of the detector.</returns>
        public static ObscuredCheatingDetector CreateDetector()
        {
            if (Instance != null) return Instance;

            if (instanceContainer == null)
            {
                instanceContainer = new GameObject(typeof(ObscuredCheatingDetector).Name);
#if !DEBUG_ENABLED
                    instanceContainer.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif
            }
            Instance = instanceContainer.AddComponent<ObscuredCheatingDetector>();
            return Instance;
        }

        /// <summary>
        /// Starts all Obscured types cheating detection for detector you have in scene.
        /// </summary>
        /// Make sure you have properly configured detector in scene with #autoStart disabled before using this method.
        public void StartDetection()
        {
            StartDetectionInternal(null);
        }

        /// <summary>
        /// Starts all Obscured types cheating detection with specified callback.
        /// </summary>
        /// If you have detector in scene make sure it has empty Detection Event.<br/>
        /// Creates a new detector instance if it doesn't exists in scene.
        /// <param name="callback">Method to call after detection.</param>
        public void StartDetection(Action callback)
        {
            StartDetectionInternal(callback);
        }

        /// <summary>
        /// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
        /// </summary>
        public void StopDetection()
        {
            StopDetectionInternal();
        }

        /// <summary>
        /// Stops and completely disposes detector component.
        /// </summary>
        /// On dispose Detector follows 2 rules:
        /// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically 
        /// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
        /// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
        /// if it has neither other components nor children attached;
        public void Dispose()
        {
            DisposeInternal();
        }
        #endregion

        #region static instance
        /// <summary>
        /// Allows reaching public properties from code.
        /// Can be null if detector does not exist in scene or if accessed at Awake phase.
        /// </summary>
        public static ObscuredCheatingDetector Instance { get; private set; }

        #endregion

        internal static bool ExistsAndIsRunning
        {
            get
            {
                return (object)Instance != null && Instance.IsRunning;
            }
        }

        private ObscuredCheatingDetector() { } // prevents direct instantiation

        #region unity messages

        private void Awake()
        {
            instancesInScene++;
            if (Init(Instance))
            {
                Instance = this;
            }

            SceneManager.sceneLoaded += OnLevelWasLoadedNew;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            instancesInScene--;
        }

        private void OnLevelWasLoadedNew(Scene scene, LoadSceneMode mode)
        {
            if (instancesInScene < 2)
            {
                if (!KeepAlive)
                {
                    DisposeInternal();
                }
            }
            else
            {
                if (!KeepAlive && Instance != this)
                {
                    DisposeInternal();
                }
            }
        }
        #endregion

        private void StartDetectionInternal(Action callback)
        {
            if (isRunning)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("already running!", this);
#endif
                return;
            }

            if (!enabled)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
#endif
                return;
            }

            if (callback == null)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
#endif
                enabled = false;
                return;
            }

            CheatDetected += callback;
            started = true;
            isRunning = true;
        }

        protected override void StartDetectionAutomatically()
        {
            StartDetectionInternal(null);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            if (Instance == this) Instance = null;
        }
    }
}

#endif
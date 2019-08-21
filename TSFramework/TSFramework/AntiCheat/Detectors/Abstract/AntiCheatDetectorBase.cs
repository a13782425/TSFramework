#if ANTI_CHEAT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TSFrame
{
    /// <summary>
    /// 反作弊探测器基类
    /// </summary>
    public abstract class AntiCheatDetectorBase : MonoBehaviour
    {
        /// <summary>
        /// 探测器游戏物体
        /// </summary>
        protected static GameObject instanceContainer;

        /// <summary>
        /// 自动启动检测器
        /// 如果为false，则你需要手动调用StartDetection()来启动它
        /// </summary>
        //[Tooltip("自动启动检测器")]
        public bool AutoStart = true;

        /// <summary>
        /// 在过场景时是否保留该检测器
        /// </summary>
        //[Tooltip("在过场景的时候是否保留")]
        public bool KeepAlive = true;

        /// <summary>
        /// 触发作弊之后是否释放控制器
        /// </summary>
        //[Tooltip("触发作弊之后是否释放控制器")]
        public bool AutoDispose = true;

        /// <summary>
        /// 发现作弊的回调
        /// </summary>
        public event Action CheatDetected;

        protected bool started;
        protected bool isRunning;

        /// <summary>
        /// 检测控制器是否在运行
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        #region Unity Method
        protected virtual void Start()
        {
            if (instanceContainer == null)
            {
                instanceContainer = gameObject;
            }

            if (AutoStart && !started)
            {
                StartDetectionAutomatically();
            }
        }

        protected virtual void OnEnable()
        {
            ResumeDetector();
        }

        protected virtual void OnDisable()
        {
            PauseDetector();
        }

        protected virtual void OnApplicationQuit()
        {
            DisposeInternal();
        }

        protected virtual void OnDestroy()
        {
            StopDetectionInternal();

            if (transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
            {
                Destroy(gameObject);
            }
            else if (GetComponentsInChildren<AntiCheatDetectorBase>().Length <= 1)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        internal virtual void OnCheatingDetected()
        {
            if (CheatDetected != null)
                CheatDetected.Invoke();

            //if (detectionEventHasListener)
            //    detectionEvent.Invoke();

            if (AutoDispose)
            {
                DisposeInternal();
            }
            else
            {
                StopDetectionInternal();
            }
        }

        protected virtual bool Init(AntiCheatDetectorBase instance)
        {
            if (instance != null && instance != this && instance.KeepAlive)
            {

                Destroy(this);
                return false;
            }

            DontDestroyOnLoad(transform.parent != null ? transform.root.gameObject : gameObject);

            return true;
        }

        protected virtual void DisposeInternal()
        {
            Destroy(this);
        }

        protected virtual bool DetectorHasCallbacks()
        {
            return CheatDetected != null;
        }

        protected virtual void StopDetectionInternal()
        {
            CheatDetected = null;
            started = false;
            isRunning = false;
        }

        protected virtual void PauseDetector()
        {
            if (!started)
                return;

            isRunning = false;
        }

        protected virtual bool ResumeDetector()
        {
            if (!started || !DetectorHasCallbacks())
                return false;

            isRunning = true;
            return true;
        }

        protected abstract void StartDetectionAutomatically();
    }
}

#endif
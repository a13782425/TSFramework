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
    /// 检测游戏加速器
    /// </summary>
    public class SpeedHackDetector : AntiCheatDetectorBase
    {

        private const long TicksPerSecond = TimeSpan.TicksPerMillisecond * 1000;
        private const int Threshold = 5000000; // == 500 ms, 真正的时间和加速时间的时间差
        private const float ThresholdFloat = 0.5f; // == 500 ms, 真正的时间和加速时间的时间差

        private static int instancesInScene;

        /// <summary>
        /// 检测器检测间隔(单位秒)
        /// </summary>
        public float Interval = 1f;

        /// <summary>
        /// 允许误报次数
        /// </summary>
        public byte MaxFalsePositives = 3;

        /// <summary>
        /// 在误报后冷却CD
        /// 设置为0禁用冷却功能
        /// </summary>
        public int CoolDown = 30;

        #region private variables
        private byte currentFalsePositives;
        private int currentCooldownShots;
        private long ticksOnStart;
        private long vulnerableTicksOnStart;
        private long previousTicks;
        private long previousIntervalTicks;
        private float vulnerableTimeOnStart;
        #endregion

        #region public static methods

        public static SpeedHackDetector CreateDetector()
        {
            if (Instance != null)
                return Instance;

            if (instanceContainer == null)
            {
                instanceContainer = new GameObject(typeof(SpeedHackDetector).Name);
#if !DEBUG_ENABLED
                instanceContainer.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
#endif
            }
            Instance = instanceContainer.AddComponent<SpeedHackDetector>();
            return Instance;
        }

        /// <summary>
        /// 启动探测器
        /// </summary>
        public void StartDetection()
        {
            this.StartDetectionInternal(null, Instance.Interval, Instance.MaxFalsePositives, Instance.CoolDown);
        }

        /// <summary>
        /// 启动探测器
        /// </summary>
        /// <param name="callback"></param>
        public void StartDetection(Action callback)
        {
            StartDetection(callback, this.Interval);
        }

        /// <summary>
        /// 启动探测器
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="interval"></param>
        public void StartDetection(Action callback, float interval)
        {
            StartDetection(callback, interval, this.MaxFalsePositives);
        }

        /// <summary>
        /// 启动探测器
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="interval"></param>
        /// <param name="maxFalsePositives"></param>
        public void StartDetection(Action callback, float interval, byte maxFalsePositives)
        {
            StartDetection(callback, interval, maxFalsePositives, this.CoolDown);
        }

        /// <summary>
        /// 启动探测器
        /// </summary>
        /// <param name="callback">发现作弊的回调</param>
        /// <param name="interval"></param>
        /// <param name="maxFalsePositives"></param>
        /// <param name="coolDown"></param>
        public void StartDetection(Action callback, float interval, byte maxFalsePositives, int coolDown)
        {
            this.StartDetectionInternal(callback, interval, maxFalsePositives, coolDown);
        }

        /// <summary>
        /// 紧停止探测器，不释放
        /// </summary>
        public void StopDetection()
        {
            this.StopDetectionInternal();
        }


        /// <summary>
        /// 停止并释放探测器
        /// </summary>
        public void Dispose()
        {
            this.DisposeInternal();
        }
        #endregion

        #region static instance

        public static SpeedHackDetector Instance { get; private set; }

        #endregion

        #region Unity Method
        private void Awake()
        {
            instancesInScene++;
            if (Init(Instance))
            {
                Instance = this;
            }

            SceneManager.sceneLoaded += OnLevelWasLoadedNew;
        }
        private void Update()
        {
            if (!isRunning)
                return;

            var reliableTicks = GetReliableTicks();
            var ticksSpentSinceLastUpdate = reliableTicks - previousTicks;

            if (ticksSpentSinceLastUpdate < 0 || ticksSpentSinceLastUpdate > TicksPerSecond)
            {
                ResetStartTicks();
                return;
            }

            previousTicks = reliableTicks;

            var intervalTicks = (long)(Interval * TicksPerSecond);

            // return if configured interval is not passed yet
            if (reliableTicks - previousIntervalTicks < intervalTicks) return;

            var reliableTicksFromStart = reliableTicks - ticksOnStart;

            var vulnerableTicks = Environment.TickCount * TimeSpan.TicksPerMillisecond;
            var vulnerableTicksFromStart = vulnerableTicks - vulnerableTicksOnStart;
            var ticksCheated = Math.Abs(vulnerableTicksFromStart - reliableTicksFromStart) > Threshold;

            var vulnerableTime = Time.realtimeSinceStartup;
            var vulnerableTimeFromStart = vulnerableTime - vulnerableTimeOnStart;
            var timeCheated = Math.Abs(reliableTicksFromStart / (float)TicksPerSecond - vulnerableTimeFromStart) > ThresholdFloat;

            if (ticksCheated || timeCheated)
            {



                currentFalsePositives++;
                if (currentFalsePositives > MaxFalsePositives)
                {

                    OnCheatingDetected();
                }
                else
                {

                    currentCooldownShots = 0;
                    ResetStartTicks();
                }
            }
            else if (currentFalsePositives > 0 && CoolDown > 0)
            {

                currentCooldownShots++;
                if (currentCooldownShots >= CoolDown)
                {
                    currentFalsePositives = 0;
                }
            }

            previousIntervalTicks = reliableTicks;
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                ResetStartTicks();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            instancesInScene--;
        }

        #endregion

        private void StartDetectionInternal(Action callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
        {
            if (isRunning)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("已经启动", this);
#endif

                return;
            }

            if (!enabled)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("游戏物体没有被启用", this);
#endif
                return;
            }


            if (callback == null)
            {
#if DEBUG_ENABLED
                Debug.LogWarning("没有作弊回调", this);
#endif
                enabled = false;
                return;
            }

            CheatDetected += callback;
            Interval = checkInterval;
            MaxFalsePositives = falsePositives;
            CoolDown = shotsTillCooldown;

            ResetStartTicks();
            currentFalsePositives = 0;
            currentCooldownShots = 0;

            started = true;
            isRunning = true;
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

        protected override void StartDetectionAutomatically()
        {
            StartDetectionInternal(null, Interval, MaxFalsePositives, CoolDown);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
            if (Instance == this) Instance = null;
        }

        private void ResetStartTicks()
        {
            ticksOnStart = GetReliableTicks();
            vulnerableTicksOnStart = Environment.TickCount * TimeSpan.TicksPerMillisecond;
            previousTicks = ticksOnStart;
            previousIntervalTicks = ticksOnStart;

            vulnerableTimeOnStart = Time.realtimeSinceStartup;
        }
        private long GetReliableTicks()
        {
            long ticks = 0;
            if (ticks == 0) ticks = DateTime.UtcNow.Ticks;
            return ticks;
        }
    }
}

#endif
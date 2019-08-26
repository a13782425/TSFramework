using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    /// <summary>
    /// 游戏设定
    /// </summary>
    internal static class GameSetting
    {
        /// <summary>
        /// 刘海屏
        /// </summary>
        private static bool _isNotchScreen = false;
        /// <summary>
        /// 刘海屏
        /// </summary>
        internal static bool IsNotchScreen { get => _isNotchScreen; set => _isNotchScreen = value; }

        private static UnityEngine.Vector2 _safeArea = UnityEngine.Vector2.zero;
        /// <summary>
        /// UI安全区域
        /// </summary>
        internal static UnityEngine.Vector2 SafeArea { get => _safeArea; set => _safeArea = value; }


        private static int _frameRate = 60;
        /// <summary>
        /// 帧率
        /// </summary>
        internal static int FrameRate { get => _frameRate; set => _frameRate = value; }

    }
}

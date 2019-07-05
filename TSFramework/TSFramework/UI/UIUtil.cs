using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public delegate void OnFloatValueChanged(float value);
    public delegate void OnStringValueChanged(string value);
    public delegate void OnBoolValueChanged(bool value);
    internal static class UIUtil
    {
        private const int FALSE = 0;
        private const int TRUE = 1;
        private static int _valueLock = 0;
        private static int _instanceId = 0;

        private static Queue<int> _instanceCacheQueue = null;
        static UIUtil()
        {
            _instanceId = int.MinValue;
            _instanceCacheQueue = new Queue<int>();
        }


        /// <summary>
        /// 获取实例ID
        /// </summary>
        /// <returns></returns>
        internal static int GetInstanceId()
        {
        Begin: if (Interlocked.CompareExchange(ref _valueLock, 1, 0) == FALSE)
            {

                int result = int.MinValue;
                if (_instanceCacheQueue.Count < 1)
                {
                    result = _instanceId;
                    _instanceId++;
                }
                else
                {
                    result = _instanceCacheQueue.Dequeue();
                }
                Interlocked.Exchange(ref _valueLock, 0);
                return result;
            }
            else
            {
                Thread.Sleep(10);
                goto Begin;
            }
        }
        /// <summary>
        /// 回收实例ID
        /// </summary>
        /// <returns></returns>
        internal static void RecoverInstanceId(int instanceId)
        {
        Begin: if (Interlocked.CompareExchange(ref _valueLock, 1, 0) == FALSE)
            {
                if (!_instanceCacheQueue.Contains(instanceId))
                    _instanceCacheQueue.Enqueue(instanceId);
                Interlocked.Exchange(ref _valueLock, 0);
            }
            else
            {
                Thread.Sleep(10);
                goto Begin;
            }
        }

        internal static T New<T>(UIView uIView, UnityEngine.EventSystems.UIBehaviour uIBehaviour) where T : UIElement
        {
            switch (uIBehaviour)
            {
                case Button button:
                    return new UIButton(uIView, button) as T;
                case Image image:
                    return new UIImage(uIView, image) as T;
                case TextPlus textPlus:
                    return new UITextPlus(uIView, textPlus) as T;
                case InputField inputField:
                    return new UIInputField(uIView, inputField) as T;
                case Text text:
                    return new UIText(uIView, text) as T;
                case Toggle toggle:
                    return new UIToggle(uIView, toggle) as T;
                case Slider slider:
                    return new UISlider(uIView, slider) as T;
                default:
                    break;
            }
            return null;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TSFrame.UI
{
    public delegate void OnIntValueChanged(int value);
    public delegate void OnFloatValueChanged(float value);
    public delegate void OnStringValueChanged(string value);
    public delegate void OnBoolValueChanged(bool value);
    internal static class UIUtil
    {
        /// <summary>
        /// 实例化UIElement
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uIView"></param>
        /// <param name="uIBehaviour"></param>
        /// <returns></returns>
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
                case Scrollbar scrollbar:
                    return new UIScrollbar(uIView, scrollbar) as T;
                case Dropdown dropdown:
                    return new UIDropdown(uIView, dropdown) as T;
                default:
                    break;
            }
            return null;

        }


        #region Event

        internal static UIEventBase GetUIEventBase(GameObject obj, EventTriggerType triggerType)
        {
            UIEventBase uIEventBase = null;
            switch (triggerType)
            {
                case EventTriggerType.PointerEnter:
                    uIEventBase = obj.AddComponent<UIPointerEnterEvent>();
                    break;
                case EventTriggerType.PointerExit:
                    uIEventBase = obj.AddComponent<UIPointerExitEvent>();
                    break;
                case EventTriggerType.PointerDown:
                    uIEventBase = obj.AddComponent<UIPointerDownEvent>();
                    break;
                case EventTriggerType.PointerUp:
                    uIEventBase = obj.AddComponent<UIPointerUpEvent>();
                    break;
                case EventTriggerType.PointerClick:
                    uIEventBase = obj.AddComponent<UIPointerClickEvent>();
                    break;
                case EventTriggerType.Drag:
                    uIEventBase = obj.AddComponent<UIDragEvent>();
                    break;
                case EventTriggerType.BeginDrag:
                    uIEventBase = obj.AddComponent<UIBeginDragEvent>();
                    break;
                case EventTriggerType.EndDrag:
                    uIEventBase = obj.AddComponent<UIEndDragEvent>();
                    break;
                case EventTriggerType.Submit:
                    uIEventBase = obj.AddComponent<UISubmitEvent>();
                    break;
                case EventTriggerType.Cancel:
                    uIEventBase = obj.AddComponent<UICancelEvent>();
                    break;
                case EventTriggerType.Drop:
                    uIEventBase = obj.AddComponent<UIDropEvent>();
                    break;
                case EventTriggerType.Scroll:
                    uIEventBase = obj.AddComponent<UIScrollEvent>();
                    break;
                case EventTriggerType.UpdateSelected:
                    uIEventBase = obj.AddComponent<UIUpdateSelectedEvent>();
                    break;
                case EventTriggerType.Select:
                    uIEventBase = obj.AddComponent<UISelectEvent>();
                    break;
                case EventTriggerType.Deselect:
                    uIEventBase = obj.AddComponent<UIDeselectEvent>();
                    break;
                case EventTriggerType.Move:
                    uIEventBase = obj.AddComponent<UIMoveEvent>();
                    break;
                case EventTriggerType.InitializePotentialDrag:
                    uIEventBase = obj.AddComponent<UIInitializePotentialDragEvent>();
                    break;
            }
            return uIEventBase;
        }

        #endregion
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TSFrame.UI
{
    internal class UIEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {
        internal event Action<PointerEventData> onBeginDrag;
        internal event Action<PointerEventData> onDrag;
        internal event Action<PointerEventData> onEndDrag;
        internal event Action<PointerEventData> onPointerEnter;
        internal event Action<PointerEventData> onPointerExit;
        internal event Action<PointerEventData> onPointDown;
        internal event Action<PointerEventData> onDrop;
        internal event Action<PointerEventData> onPointerUp;
        internal event Action<PointerEventData> onPointerClick;
        internal event Action<PointerEventData> onInitializePotentialDrag;
        internal event Action<PointerEventData> onScroll;
        internal event Action<BaseEventData> onUpdateSelected;
        internal event Action<BaseEventData> onSelect;
        internal event Action<BaseEventData> onDeselect;
        internal event Action<BaseEventData> onSubmit;
        internal event Action<BaseEventData> onCancel;
        internal event Action<AxisEventData> onMove;
        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointDown?.Invoke(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            onDrop?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke(eventData);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDrag?.Invoke(eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            onScroll?.Invoke(eventData);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            onUpdateSelected?.Invoke(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
            onSelect?.Invoke(eventData);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            onDeselect?.Invoke(eventData);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onSubmit?.Invoke(eventData);
        }

        public void OnCancel(BaseEventData eventData)
        {
            onCancel?.Invoke(eventData);
        }
        public void OnMove(AxisEventData eventData)
        {
            onMove?.Invoke(eventData);
        }
    }
    internal class UIEventBase : MonoBehaviour
    {
        internal Action<PointerEventData> pointerCallback;
        internal Action<BaseEventData> baseCallback;
        internal Action<AxisEventData> axisCallback;
    }
    //IPointerEnterHandler
    internal class UIPointerEnterEvent : UIEventBase, IPointerEnterHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IPointerExitHandler
    internal class UIPointerExitEvent : UIEventBase, IPointerExitHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IPointerDownHandler
    internal class UIPointerDownEvent : UIEventBase, IPointerDownHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IPointerUpHandler
    internal class UIPointerUpEvent : UIEventBase, IPointerUpHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IPointerClickHandler
    internal class UIPointerClickEvent : UIEventBase, IPointerClickHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnPointerClick(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IInitializePotentialDragHandler
    internal class UIInitializePotentialDragEvent : UIEventBase, IInitializePotentialDragHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IBeginDragHandler
    internal class UIBeginDragEvent : UIEventBase, IBeginDragHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnBeginDrag(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IDragHandler
    internal class UIDragEvent : UIEventBase, IDragHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnDrag(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IEndDragHandler
    internal class UIEndDragEvent : UIEventBase, IEndDragHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnEndDrag(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IDropHandler
    internal class UIDropEvent : UIEventBase, IDropHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnDrop(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IScrollHandler
    internal class UIScrollEvent : UIEventBase, IScrollHandler
    {
        //internal event Action<PointerEventData> callback;

        public void OnScroll(PointerEventData eventData)
        {
            pointerCallback?.Invoke(eventData);
        }
    }
    //IUpdateSelectedHandler
    internal class UIUpdateSelectedEvent : UIEventBase, IUpdateSelectedHandler
    {
        //internal event Action<BaseEventData> callback;

        public void OnUpdateSelected(BaseEventData eventData)
        {
            baseCallback?.Invoke(eventData);
        }
    }
    //ISelectHandler
    internal class UISelectEvent : UIEventBase, ISelectHandler
    {
        //internal event Action<BaseEventData> callback;

        public void OnSelect(BaseEventData eventData)
        {
            baseCallback?.Invoke(eventData);
        }
    }
    //IDeselectHandler
    internal class UIDeselectEvent : UIEventBase, IDeselectHandler
    {
        //internal event Action<BaseEventData> callback;

        public void OnDeselect(BaseEventData eventData)
        {
            baseCallback?.Invoke(eventData); ;
        }
    }
    //IMoveHandler
    internal class UIMoveEvent : UIEventBase, IMoveHandler
    {
        //internal event Action<AxisEventData> callback;

        public void OnMove(AxisEventData eventData)
        {
            axisCallback?.Invoke(eventData);
        }
    }
    //ISubmitHandler
    internal class UISubmitEvent : UIEventBase, ISubmitHandler
    {
        //internal event Action<BaseEventData> callback;

        public void OnSubmit(BaseEventData eventData)
        {
            baseCallback?.Invoke(eventData);
        }
    }
    //ICancelHandler
    internal class UICancelEvent : UIEventBase, ICancelHandler
    {
        //internal event Action<BaseEventData> callback;

        public void OnCancel(BaseEventData eventData)
        {
            baseCallback?.Invoke(eventData);
        }
    }
}

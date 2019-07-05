using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TSFrame.Module
{
    internal sealed class DefaultUIModule : BaseModule<DefaultUIModule>, IUILoader
    {
        private Canvas _canvas = null;
        private GameObject _eventSystem = null;
        private GameObject _mainObj = null;
        public GameObject MainUIObj => _mainObj;

        private Dictionary<UILayerEnum, RectTransform> _tranDic = null;

        private Dictionary<int, UIPanel> _panelDic = null;

        private List<UIPanel> _panelList = null;

        //private List<UIPanel> _needClosePanel = null;

        public override void Init()
        {
            base.Init();
            CreateMainObj();
            CreateLayerObj();
            _panelDic = new Dictionary<int, UIPanel>();
            _panelList = new List<UIPanel>();
            //_needClosePanel = new List<UIPanel>();
        }
        private void CreateMainObj()
        {
            _mainObj = new GameObject("MainUI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _mainObj.transform.SetParent(this.transform);
            _canvas = _mainObj.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _tranDic = new Dictionary<UILayerEnum, RectTransform>();
            _eventSystem = new GameObject("System", typeof(EventSystem), typeof(StandaloneInputModule));
            _eventSystem.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _eventSystem.transform.SetParent(this.transform);
        }
        private void CreateLayerObj()
        {
            Type enumType = typeof(UILayerEnum);
            string[] strs = Enum.GetNames(enumType);
            foreach (var item in strs)
            {
                UILayerEnum uILayerEnum = (UILayerEnum)Enum.Parse(enumType, item);
                GameObject obj = new GameObject(uILayerEnum.ToString(), typeof(RectTransform));
                obj.transform.SetParent(MainUIObj.transform);
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.SetSiblingIndex((int)uILayerEnum);
                _tranDic.Add(uILayerEnum, rectTransform);
            }
        }
        public T CreatePanel<T>() where T : UIPanel, new()
        {
            T t = UIView.CreateView<T>();
            t.transform.SetParent(_tranDic[t.LayerEnum]);
            _panelDic.Add(t.InstanceId, t);
            _panelList.Add(t);
            return t;
        }
        public T ShowPanel<T>() where T : UIPanel, new()
        {
            UIPanel panel = null;
            foreach (var item in _panelList)
            {
                if (item is T)
                {
                    panel = item;
                    break;
                }
            }
            if (panel == null)
            {
                panel = CreatePanel<T>();
            }
            else
            {
                panel.active = true;
            }
            return panel as T;
        }
        public T HidePanel<T>() where T : UIPanel, new()
        {
            UIPanel panel = null;
            foreach (var item in _panelList)
            {
                if (item is T)
                {
                    panel = item;
                    break;
                }
            }
            if (panel == null)
            {
                return null;
            }
            panel.active = false;
            return panel as T;
        }
        public void ClosePanel(UIPanel uIPanel)
        {
            if (uIPanel != null)
            {
                _panelList.Remove(uIPanel);
                _panelDic.Remove(uIPanel.InstanceId);
                if (uIPanel.gameObject != null)
                {
                    uIPanel.Close();
                }
            }
        }
        public void ClosePanel<T>() where T : UIPanel, new()
        {
            List<UIPanel> tempList = new List<UIPanel>();
            foreach (var item in _panelList)
            {
                if (item is T || item == null)
                {
                    tempList.Add(item);
                }
            }
            foreach (var item in tempList)
            {
                _panelList.Remove(item);
                _panelDic.Remove(item.InstanceId);
                if (item.gameObject != null)
                {
                    item.Close();
                }
            }
        }

        public Transform GetParent(UILayerEnum uILayerEnum)
        {
            if (_tranDic.ContainsKey(uILayerEnum))
            {
                return _tranDic[uILayerEnum];
            }
            else
            {
                return null;
            }
        }

        public override void Freed()
        {
            foreach (var item in _panelList)
            {
                if (item != null)
                {
                    item.Close();
                }
            }

            _panelList.Clear();
            _panelDic.Clear();
            Object.Destroy(this.gameObject);
        }

    }
}

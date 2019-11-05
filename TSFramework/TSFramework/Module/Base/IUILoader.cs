using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.UI;
using UnityEngine;

namespace TSFrame.Module
{
    public interface IUILoader
    {
        Canvas MainCanvas { get; }
        GameObject MainUIObj { get; }
        /// <summary>
        /// 永远创建新的面板
        /// 如果parent == null,则按照UILayerEnum进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T CreatePanel<T>() where T : UIPanel, new();

        /// <summary>
        /// 显示一个Panel，如果没有则创建，如果有则显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T ShowPanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 获取一个Panel，如果没有则创建，如果有则返回（不会改变其状态）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetPanel<T>() where T : UIPanel, new();

        /// <summary>
        /// 获得全部T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> GetAllPanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 获得全部窗口
        /// </summary>
        /// <returns></returns>
        List<UIPanel> GetAllPanel();

        /// <summary>
        /// 判断某个类型的Panel是否已经被打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IsOpen<T>() where T : UIPanel, new();

        /// <summary>
        /// 隐藏一个Panel
        /// </summary>
        /// <param name="uIPanel"></param>
        void HidePanel(UIPanel uIPanel);
        /// <summary>
        /// 隐藏一个类型Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void HidePanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 隐藏对应层级的UI
        /// </summary>
        /// <param name="layerEnum"></param>
        void HidePanelByLayer(UILayerEnum layerEnum);
        /// <summary>
        /// 隐藏全部面板
        /// </summary>
        void HideAllPanel();

        /// <summary>
        /// 关闭一个Panel
        /// </summary>
        /// <param name="uIPanel"></param>
        void ClosePanel(UIPanel uIPanel);
        /// <summary>
        /// 关闭一个类型的panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ClosePanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 关闭对应层级的UI
        /// </summary>
        /// <param name="layerEnum"></param>
        void ClosePanelByLayer(UILayerEnum layerEnum);
        /// <summary>
        /// 关闭全部面板
        /// </summary>
        void CloseAllPanel();

        /// <summary>
        /// 根据层级获取父物体
        /// </summary>
        /// <param name="uILayerEnum"></param>
        /// <returns></returns>
        Transform GetParent(UILayerEnum uILayerEnum);
    }
}

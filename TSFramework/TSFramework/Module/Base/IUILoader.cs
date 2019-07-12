﻿using System;
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
        GameObject MainUIObj { get; }
        /// <summary>
        /// 如果parent == null,则按照UILayerEnum进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        T CreatePanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 显示一个Panel，如果没有则创建，如果有则显示
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        T ShowPanel<T>() where T : UIPanel, new();
        /// <summary>
        /// 隐藏一个Panel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T HidePanel<T>() where T : UIPanel, new();
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

        Transform GetParent(UILayerEnum uILayerEnum);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TSFrame.Module
{
    internal class DefaultResourcesModule : BaseModule<DefaultResourcesModule>, IResourcesLoader
    {
        private Dictionary<string, ResourcesDto> _resourcesCacheDic;

        private List<string> _destroyTempList;
        /// <summary>
        /// 每次检测释放没用资源的时间（秒）
        /// </summary>
        public int UnloadTime { get; set; }
        /// <summary>
        /// 释放资源临时计时变量
        /// </summary>
        private float _unloadTempTime = 0;
        /// <summary>
        /// 删除Resources的时间（秒）
        /// </summary>
        public int DestroyTime { get; set; }
        public override void Init()
        {
            _resourcesCacheDic = new Dictionary<string, ResourcesDto>();
            _destroyTempList = new List<string>();
            UnloadTime = 180;
            DestroyTime = 180;
        }
        /// <summary>
        /// 读取并创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadOrCreate<T>(string path, bool autoRelease = true) where T : Object
        {
            T resObj = Load<T>(path, autoRelease);
            if (resObj != null)
            {
                return Object.Instantiate(resObj);
            }
            return null;
        }
        /// <summary>
        /// 读取并创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object LoadOrCreate(string path, bool autoRelease = true)
        {
            Object resObj = Load(path, autoRelease);
            if (resObj != null)
            {
                return Object.Instantiate(resObj);
            }
            return null;
        }
        /// <summary>
        /// 读取不创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path, bool autoRelease = true) where T : Object
        {
            ResourcesDto resourcesDto;
            if (!_resourcesCacheDic.ContainsKey(path))
            {
                Object resT = Resources.Load<T>(path);
                resourcesDto = new ResourcesDto();
                resourcesDto.ResourceObj = resT;
                resourcesDto.AutoRelease = autoRelease;
            }
            else
            {
                resourcesDto = _resourcesCacheDic[path];
            }
            resourcesDto.LastUseTime = GameApp.Instance.gameTime;
            return resourcesDto.ResourceObj as T;
        }
        /// <summary>
        /// 读取不创建
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object Load(string path, bool autoRelease = true)
        {
            ResourcesDto resourcesDto;
            if (!_resourcesCacheDic.ContainsKey(path))
            {
                Object resT = Resources.Load(path);
                resourcesDto = new ResourcesDto();
                resourcesDto.ResourceObj = resT;
                resourcesDto.AutoRelease = autoRelease;
            }
            else
            {
                resourcesDto = _resourcesCacheDic[path];
            }
            resourcesDto.LastUseTime = GameApp.Instance.gameTime;
            return resourcesDto.ResourceObj;
        }
        public override void Update(float deltaTime)
        {
            if (_unloadTempTime > UnloadTime)
            {

                foreach (var item in _resourcesCacheDic)
                {
                    if (item.Value.IsCanDestroy)
                    {
                        _destroyTempList.Add(item.Key);
                    }
                }
                foreach (var item in _destroyTempList)
                {
                    ResourcesDto resourcesDto = _resourcesCacheDic[item];
                    Object.Destroy(resourcesDto.ResourceObj);
                    _resourcesCacheDic.Remove(item);
                }
                _destroyTempList.Clear();
                //间隔一段时间调用
                Resources.UnloadUnusedAssets();
                _unloadTempTime = 0;
            }
            _unloadTempTime += deltaTime;
        }

        public override void Freed()
        {

        }

        private class ResourcesDto
        {
            public Object ResourceObj = null;
            public float LastUseTime = 0;
            public bool AutoRelease = true;

            public bool IsCanDestroy => AutoRelease ? false : (GameApp.Instance.gameTime - this.LastUseTime) > Instance.DestroyTime;
        }
    }
}

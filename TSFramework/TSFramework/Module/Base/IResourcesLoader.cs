using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSFrame.Module;
using Object = UnityEngine.Object;

namespace TSFrame.Module
{
    public interface IResourcesLoader
    {
        T LoadOrCreate<T>(string path, bool autoRelease = true) where T : Object;
        Object LoadOrCreate(string path, bool autoRelease = true);
        T Load<T>(string path, bool autoRelease = true) where T : Object;
        Object Load(string path, bool autoRelease = true);
    }
}

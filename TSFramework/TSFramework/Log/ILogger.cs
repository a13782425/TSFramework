using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    public interface ILogger
    {
        bool IsEnable { get; set; }

        void Log(string message);
        void Log(object message);
        void LogWarning(string message);
        void LogWarning(object message);
        void LogError(string message);
        void LogError(object message);
    }
}

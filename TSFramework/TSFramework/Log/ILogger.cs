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
        void LogWarn(string message);
        void LogWarn(object message);
        void LogError(string message);
        void LogError(object message);
    }
}

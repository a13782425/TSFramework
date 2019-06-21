﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    public sealed class DefaultLogger : ILogger
    {
        public DefaultLogger()
        {
            IsEnable = true;
        }

        public bool IsEnable { get; set; }

        public void Log(string message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
            }
        }

        public void Log(object message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);
            }
        }

        public void LogWarn(string message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
            }
        }

        public void LogWarn(object message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
            }
        }

        public void LogError(string message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
            }
        }

        public void LogError(object message)
        {
            if (IsEnable)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
            }
        }
    }
}
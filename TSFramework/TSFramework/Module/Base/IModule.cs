﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame.Module
{
    public interface IModule
    {
        void Init();
        void Update(float deltaTime);
        void Freed();
    }
}

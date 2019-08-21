#if ANTI_CHEAT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    [Serializable]
    internal struct Byte4
    {
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;

        /// <summary>
        /// 混淆
        /// </summary>
        public void Shuffle()
        {
            var buffer = b2;
            b2 = b3;
            b3 = buffer;
        }
        /// <summary>
        /// 反混淆
        /// </summary>
        public void UnShuffle()
        {
            var buffer = b3;
            b3 = b2;
            b2 = buffer;
        }
    }
}

#endif
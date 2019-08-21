#if ANTI_CHEAT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    internal struct Byte8
    {
        public byte b1;
        public byte b2;
        public byte b3;
        public byte b4;
        public byte b5;
        public byte b6;
        public byte b7;
        public byte b8;

        /// <summary>
        /// 混淆
        /// </summary>
        public void Shuffle()
        {
            var buffer = b1;
            b1 = b2;
            b2 = buffer;
            buffer = b5;
            b5 = b6;
            var buffer2 = b8;
            b8 = buffer;
            b6 = buffer2;
        }

        /// <summary>
        /// 反混淆
        /// </summary>
        public void UnShuffle()
        {
            var buffer = b1;
            b1 = b2;
            b2 = buffer;
            buffer = b5;
            b5 = b8;
            var buffer2 = b6;
            b6 = buffer;
            b8 = buffer2;
        }
    }
}

#endif
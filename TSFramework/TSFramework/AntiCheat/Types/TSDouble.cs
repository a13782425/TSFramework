#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !ENABLE_IL2CPP
#define NO_IL2CPP
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace TSFrame
{
    [Serializable]
    public struct TSDouble : IFormattable, IEquatable<TSDouble>, IComparable<TSDouble>, IComparable<double>, IComparable
    {
        private static long cryptoKey = 210987L;

        [SerializeField]
        private long currentCryptoKey;

        [SerializeField]
        private long hiddenValue;

        [SerializeField]
        [FormerlySerializedAs("hiddenValue")]
#pragma warning disable 414
        private Byte8 hiddenValueOldByte8;
#pragma warning restore 414

        [SerializeField]
        private bool inited;

        [SerializeField]
        private double fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSDouble(double value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = InternalEncrypt(value);
            hiddenValueOldByte8 = default(Byte8);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
			migratedVersion = null;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : 0L;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(long newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Encrypt(double value)
        {
            return Encrypt(value, cryptoKey);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long Encrypt(double value, long key)
        {
            var u = new DoubleLongBytesUnion { d = value };
            u.l = u.l ^ key;
            u.b8.Shuffle();
            return u.l;
        }

        private static long InternalEncrypt(double value, long key = 0L)
        {
            var currentKey = key;
            if (currentKey == 0L)
            {
                currentKey = cryptoKey;
            }

            return Encrypt(value, currentKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static double Decrypt(long value)
        {
            return Decrypt(value, cryptoKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static double Decrypt(long value, long key)
        {
            var u = new DoubleLongBytesUnion { l = value };
            u.b8.UnShuffle();
            u.l ^= key;
            return u.d;
        }

        public static TSDouble FromEncrypted(long encrypted)
        {
            var instance = new TSDouble();
            instance.SetEncrypted(encrypted);
            return instance;
        }

        /// <summary>
        /// 在SetNewCryptoKey()之后使用
        /// 使用新的密钥加密当前实例
        /// </summary>
        public void ApplyNewCryptoKey()
        {
            if (currentCryptoKey != cryptoKey)
            {
                hiddenValue = InternalEncrypt(InternalDecrypt(), cryptoKey);
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// 使用随机的新密钥
        /// </summary>
        public void RandomizeCryptoKey()
        {
            var decrypted = InternalDecrypt();
            currentCryptoKey = ThreadSafeRandom.Next(100000, 999999);
            hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public long GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(long encrypted)
        {
            inited = true;
            hiddenValue = encrypted;

            if (currentCryptoKey == 0)
            {
                currentCryptoKey = cryptoKey;
            }

            if (ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValueActive = false;
                fakeValue = InternalDecrypt();
                fakeValueActive = true;
            }
            else
            {
                fakeValueActive = false;
            }
        }

        /// <summary>
        /// 获取原值
        /// </summary>
        /// <returns></returns>
        public double GetDecrypted()
        {
            return InternalDecrypt();
        }

        private double InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = InternalEncrypt(0);
                fakeValue = 0;
                fakeValueActive = false;
                inited = true;

                return 0;
            }

            var decrypted = Decrypt(hiddenValue, currentCryptoKey);
            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue) > ObscuredCheatingDetector.Instance.DoubleEpsilon)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region operators, overrides, interface implementations

        //! @cond
        public static implicit operator TSDouble(double value)
        {
            return new TSDouble(value);
        }

        public static implicit operator double(TSDouble value)
        {
            return value.InternalDecrypt();
        }

        public static explicit operator TSDouble(TSFloat f)
        {
            return (float)f;
        }

        public static TSDouble operator ++(TSDouble input)
        {
            var decrypted = input.InternalDecrypt() + 1d;
            input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                input.fakeValue = decrypted;
                input.fakeValueActive = true;
            }
            else
            {
                input.fakeValueActive = false;
            }

            return input;
        }

        public static TSDouble operator --(TSDouble input)
        {
            var decrypted = input.InternalDecrypt() - 1d;
            input.hiddenValue = InternalEncrypt(decrypted, input.currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                input.fakeValue = decrypted;
                input.fakeValueActive = true;
            }
            else
            {
                input.fakeValueActive = false;
            }

            return input;
        }

        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }

        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }

        public string ToString(string format)
        {
            return InternalDecrypt().ToString(format);
        }

        public string ToString(IFormatProvider provider)
        {
            return InternalDecrypt().ToString(provider);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return InternalDecrypt().ToString(format, provider);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TSDouble))
                return false;
            return Equals((TSDouble)obj);
        }

        public bool Equals(TSDouble obj)
        {
            return obj.InternalDecrypt().Equals(InternalDecrypt());
        }

        public int CompareTo(TSDouble other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(double other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is double)) throw new ArgumentException("Argument must be double");
			return CompareTo((double)obj);
#endif
        }

        #endregion

        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleLongBytesUnion
        {
            [FieldOffset(0)]
            public double d;

            [FieldOffset(0)]
            public long l;

            [FieldOffset(0)]
            public Byte8 b8;
        }
    }
}

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

namespace TSFrame
{
    [Serializable]
    public struct TSDecimal : IFormattable, IEquatable<TSDecimal>, IComparable<TSDecimal>, IComparable<decimal>, IComparable
    {
        private static long cryptoKey = 209208L;

        [SerializeField]
        private long currentCryptoKey;

        [SerializeField]
        private Byte16 hiddenValue;

        [SerializeField]
        private bool inited;

        private decimal fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSDecimal(decimal value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = InternalEncrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : 0m;
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
        public static decimal Encrypt(decimal value)
        {
            return Encrypt(value, cryptoKey);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal Encrypt(decimal value, long key)
        {
            var u = new DecimalLongBytesUnion();
            u.d = value;
            u.l1 = u.l1 ^ key;
            u.l2 = u.l2 ^ key;

            return u.d;
        }

        private static Byte16 InternalEncrypt(decimal value)
        {
            return InternalEncrypt(value, 0L);
        }

        private static Byte16 InternalEncrypt(decimal value, long key)
        {
            var currentKey = key;
            if (currentKey == 0L)
            {
                currentKey = cryptoKey;
            }

            var union = new DecimalLongBytesUnion();
            union.d = value;
            union.l1 = union.l1 ^ currentKey;
            union.l2 = union.l2 ^ currentKey;

            return union.b16;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static decimal Decrypt(decimal value)
        {
            return Decrypt(value, cryptoKey);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static decimal Decrypt(decimal value, long key)
        {
            var u = new DecimalLongBytesUnion();
            u.d = value;
            u.l1 = u.l1 ^ key;
            u.l2 = u.l2 ^ key;
            return u.d;
        }

        public static TSDecimal FromEncrypted(decimal encrypted)
        {
            var instance = new TSDecimal();
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
            currentCryptoKey = ThreadSafeRandom.Next();
            hiddenValue = InternalEncrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public decimal GetEncrypted()
        {
            ApplyNewCryptoKey();

            var union = new DecimalLongBytesUnion();
            union.b16 = hiddenValue;

            return union.d;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(decimal encrypted)
        {
            inited = true;
            var union = new DecimalLongBytesUnion();
            union.d = encrypted;

            hiddenValue = union.b16;

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
        public decimal GetDecrypted()
        {
            return InternalDecrypt();
        }

        private decimal InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = InternalEncrypt(0m);
                fakeValue = 0m;
                fakeValueActive = false;
                inited = true;

                return 0m;
            }

            var union = new DecimalLongBytesUnion();
            union.b16 = hiddenValue;

            union.l1 = union.l1 ^ currentCryptoKey;
            union.l2 = union.l2 ^ currentCryptoKey;

            var decrypted = union.d;

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region overrides

        public static implicit operator TSDecimal(decimal value)
        {
            return new TSDecimal(value);
        }

        public static implicit operator decimal(TSDecimal value)
        {
            return value.InternalDecrypt();
        }

        public static explicit operator TSDecimal(TSFloat f)
        {
            return (decimal)(float)f;
        }

        public static TSDecimal operator ++(TSDecimal input)
        {
            var decrypted = input.InternalDecrypt() + 1m;
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

        public static TSDecimal operator --(TSDecimal input)
        {
            var decrypted = input.InternalDecrypt() - 1m;
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
            if (!(obj is TSDecimal))
                return false;
            return Equals((TSDecimal)obj);
        }

        public bool Equals(TSDecimal obj)
        {
            return obj.InternalDecrypt().Equals(InternalDecrypt());
        }

        public int CompareTo(TSDecimal other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(decimal other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is decimal)) throw new ArgumentException("Argument must be decimal");
			return CompareTo((decimal)obj);
#endif
        }

        #endregion

        [StructLayout(LayoutKind.Explicit)]
        private struct DecimalLongBytesUnion
        {
            [FieldOffset(0)]
            public decimal d;

            [FieldOffset(0)]
            public long l1;

            [FieldOffset(8)]
            public long l2;

            [FieldOffset(0)]
            public Byte16 b16;
        }
    }
}

#if ANTI_CHEAT
#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !ENABLE_IL2CPP
#define NO_IL2CPP
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TSFrame
{
    [Serializable]
    public struct TSUInt : IFormattable, IEquatable<TSUInt>, IComparable<TSUInt>, IComparable<uint>, IComparable
    {
        private static uint cryptoKey = 240513;


        [SerializeField]
        private uint currentCryptoKey;

        [SerializeField]
        private uint hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private uint fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSUInt(uint value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : 0;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(uint newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint Encrypt(uint value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static uint Encrypt(uint value, uint key)
        {
            if (key == 0)
            {
                return value ^ cryptoKey;
            }
            return value ^ key;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static uint Decrypt(uint value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static uint Decrypt(uint value, uint key)
        {
            if (key == 0)
            {
                return value ^ cryptoKey;
            }
            return value ^ key;
        }

        public static TSUInt FromEncrypted(uint encrypted)
        {
            var instance = new TSUInt();
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
                hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// 使用随机的新密钥
        /// </summary>
        public void RandomizeCryptoKey()
        {
            var decrypted = InternalDecrypt();

            currentCryptoKey = (uint)ThreadSafeRandom.Next();
            hiddenValue = Encrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public uint GetEncrypted()
        {
            ApplyNewCryptoKey();

            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(uint encrypted)
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
        public uint GetDecrypted()
        {
            return InternalDecrypt();
        }

        private uint InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(0);
                fakeValue = 0;
                fakeValueActive = false;
                inited = true;

                return 0;
            }

            var decrypted = Decrypt(hiddenValue, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region overrides

        public static implicit operator TSUInt(uint value)
        {
            return new TSUInt(value);
        }

        public static implicit operator uint(TSUInt value)
        {
            return value.InternalDecrypt();
        }

        public static explicit operator TSInt(TSUInt value)
        {
            return (int)value.InternalDecrypt();
        }

        public static TSUInt operator ++(TSUInt input)
        {
            var decrypted = input.InternalDecrypt() + 1;
            input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

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

        public static TSUInt operator --(TSUInt input)
        {
            var decrypted = input.InternalDecrypt() - 1;
            input.hiddenValue = Encrypt(decrypted, input.currentCryptoKey);

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
            if (!(obj is TSUInt))
                return false;
            return Equals((TSUInt)obj);
        }

        public bool Equals(TSUInt obj)
        {
            if (currentCryptoKey == obj.currentCryptoKey)
            {
                return hiddenValue == obj.hiddenValue;
            }

            return Decrypt(hiddenValue, currentCryptoKey) == Decrypt(obj.hiddenValue, obj.currentCryptoKey);
        }

        public int CompareTo(TSUInt other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(uint other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is uint)) throw new ArgumentException("Argument must be uint");
			return CompareTo((uint)obj);
#endif
        }

        #endregion
    }
}

#endif
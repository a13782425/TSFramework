#if ANTI_CHEAT
#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !ENABLE_IL2CPP
#define NO_IL2CPP
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSFrame
{
    [Serializable]
    public struct TSUShort : IFormattable, IEquatable<TSUShort>, IComparable<TSUShort>, IComparable<ushort>, IComparable
    {
        private static ushort cryptoKey = 224;

        private ushort currentCryptoKey;
        private ushort hiddenValue;
        private bool inited;

        private ushort fakeValue;
        private bool fakeValueActive;

        private TSUShort(ushort value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = EncryptDecrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : (ushort)0;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(ushort newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加解密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort EncryptDecrypt(ushort value)
        {
            return EncryptDecrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加解密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ushort EncryptDecrypt(ushort value, ushort key)
        {
            if (key == 0)
            {
                return (ushort)(value ^ cryptoKey);
            }
            return (ushort)(value ^ key);
        }

        public static TSUShort FromEncrypted(ushort encrypted)
        {
            var instance = new TSUShort();
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
                hiddenValue = EncryptDecrypt(InternalDecrypt(), cryptoKey);
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// 使用随机的新密钥
        /// </summary>
        public void RandomizeCryptoKey()
        {
            var decrypted = InternalDecrypt();

            currentCryptoKey = (ushort)ThreadSafeRandom.Next(short.MaxValue);
            hiddenValue = EncryptDecrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public ushort GetEncrypted()
        {
            ApplyNewCryptoKey();

            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(ushort encrypted)
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
        public ushort GetDecrypted()
        {
            return InternalDecrypt();
        }

        private ushort InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = EncryptDecrypt(0);
                fakeValue = 0;
                fakeValueActive = false;
                inited = true;

                return 0;
            }

            var decrypted = EncryptDecrypt(hiddenValue, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region overrides

        public static implicit operator TSUShort(ushort value)
        {
            return new TSUShort(value);
        }

        public static implicit operator ushort(TSUShort value)
        {
            return value.InternalDecrypt();
        }

        public static TSUShort operator ++(TSUShort input)
        {
            var decrypted = (ushort)(input.InternalDecrypt() + 1);
            input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

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

        public static TSUShort operator --(TSUShort input)
        {
            var decrypted = (ushort)(input.InternalDecrypt() - 1);
            input.hiddenValue = EncryptDecrypt(decrypted, input.currentCryptoKey);

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
            if (!(obj is TSUShort))
                return false;
            return Equals((TSUShort)obj);
        }

        public bool Equals(TSUShort obj)
        {
            if (currentCryptoKey == obj.currentCryptoKey)
            {
                return hiddenValue == obj.hiddenValue;
            }

            return EncryptDecrypt(hiddenValue, currentCryptoKey) == EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
        }

        public int CompareTo(TSUShort other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(ushort other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is ushort)) throw new ArgumentException("Argument must be ushort");
			return CompareTo((ushort)obj);
#endif
        }

        #endregion

    }
}

#endif
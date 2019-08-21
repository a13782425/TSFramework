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
    public struct TSSByte : IFormattable, IEquatable<TSSByte>, IComparable<TSSByte>, IComparable<sbyte>, IComparable
    {
        private static sbyte cryptoKey = 112;

        private sbyte currentCryptoKey;
        private sbyte hiddenValue;
        private bool inited;

        private sbyte fakeValue;
        private bool fakeValueActive;

        private TSSByte(sbyte value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = EncryptDecrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : (sbyte)0;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(sbyte newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加解密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static sbyte EncryptDecrypt(sbyte value)
        {
            return EncryptDecrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加解密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static sbyte EncryptDecrypt(sbyte value, sbyte key)
        {
            if (key == 0)
            {
                return (sbyte)(value ^ cryptoKey);
            }
            return (sbyte)(value ^ key);
        }

        public static TSSByte FromEncrypted(sbyte encrypted)
        {
            var instance = new TSSByte();
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
            currentCryptoKey = (sbyte)ThreadSafeRandom.Next(sbyte.MaxValue);
            hiddenValue = EncryptDecrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public sbyte GetEncrypted()
        {
            ApplyNewCryptoKey();

            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(sbyte encrypted)
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
        public sbyte GetDecrypted()
        {
            return InternalDecrypt();
        }

        private sbyte InternalDecrypt()
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

        public static implicit operator TSSByte(sbyte value)
        {
            return new TSSByte(value);
        }

        public static implicit operator sbyte(TSSByte value)
        {
            return value.InternalDecrypt();
        }

        public static TSSByte operator ++(TSSByte input)
        {
            var decrypted = (sbyte)(input.InternalDecrypt() + 1);
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

        public static TSSByte operator --(TSSByte input)
        {
            var decrypted = (sbyte)(input.InternalDecrypt() - 1);
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
            if (!(obj is TSSByte))
                return false;
            return Equals((TSSByte)obj);
        }

        public bool Equals(TSSByte obj)
        {
            if (currentCryptoKey == obj.currentCryptoKey)
            {
                return hiddenValue == obj.hiddenValue;
            }

            return EncryptDecrypt(hiddenValue, currentCryptoKey) == EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
        }

        public int CompareTo(TSSByte other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(sbyte other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is sbyte)) throw new ArgumentException("Argument must be sbyte");
			return CompareTo((sbyte)obj);
#endif
        }
        #endregion
    }
}

#endif
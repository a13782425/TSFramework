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
    public struct TSChar : IEquatable<TSChar>, IComparable<TSChar>, IComparable<char>, IComparable
    {
        private static char cryptoKey = '\x2014';

        private char currentCryptoKey;
        private char hiddenValue;
        private bool inited;

        private char fakeValue;
        private bool fakeValueActive;

        private TSChar(char value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = EncryptDecrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : '\0';
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }
        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(char newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static char EncryptDecrypt(char value)
        {
            return EncryptDecrypt(value, '\0');
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static char EncryptDecrypt(char value, char key)
        {
            if (key == '\0')
            {
                return (char)(value ^ cryptoKey);
            }
            return (char)(value ^ key);
        }

        public static TSChar FromEncrypted(char encrypted)
        {
            var instance = new TSChar();
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
            currentCryptoKey = (char)ThreadSafeRandom.Next(char.MaxValue);
            hiddenValue = EncryptDecrypt(decrypted, currentCryptoKey);
        }
        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public char GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }
        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(char encrypted)
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
        public char GetDecrypted()
        {
            return InternalDecrypt();
        }

        private char InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = EncryptDecrypt('\0');
                fakeValue = '\0';
                fakeValueActive = false;
                inited = true;

                return '\0';
            }

            var decrypted = EncryptDecrypt(hiddenValue, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region overrides

        public static implicit operator TSChar(char value)
        {
            return new TSChar(value);
        }

        public static implicit operator char(TSChar value)
        {
            return value.InternalDecrypt();
        }

        public static TSChar operator ++(TSChar input)
        {
            var decrypted = (char)(input.InternalDecrypt() + 1);
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

        public static TSChar operator --(TSChar input)
        {
            var decrypted = (char)(input.InternalDecrypt() - 1);
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
#if !UNITY_WINRT
        public string ToString(IFormatProvider provider)
        {
            return InternalDecrypt().ToString(provider);
        }
#endif

        public override bool Equals(object obj)
        {
            if (!(obj is TSChar))
                return false;
            return Equals((TSChar)obj);
        }
        public bool Equals(TSChar obj)
        {
            if (currentCryptoKey == obj.currentCryptoKey)
            {
                return hiddenValue == obj.hiddenValue;
            }

            return EncryptDecrypt(hiddenValue, currentCryptoKey) == EncryptDecrypt(obj.hiddenValue, obj.currentCryptoKey);
        }

        public int CompareTo(TSChar other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(char other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is char)) throw new ArgumentException("Argument must be char");
			return CompareTo((char)obj);
#endif
        }
        #endregion
    }
}

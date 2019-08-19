#if (UNITY_WINRT || UNITY_WINRT_10_0 || UNITY_WSA || UNITY_WSA_10_0) && !ENABLE_IL2CPP
#define NO_IL2CPP
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace TSFrame
{
    [Serializable]
    public struct TSBool : IEquatable<TSBool>, IComparable<TSBool>, IComparable<bool>, IComparable
    {
        private static byte cryptoKey = 215;
        [SerializeField]
        private byte currentCryptoKey;

        [SerializeField]
        private int hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private bool fakeValue;

        [SerializeField]
        [FormerlySerializedAs("fakeValueChanged")]
        private bool fakeValueActive;

        private TSBool(bool value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : false;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(byte newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 使用默认密钥加密Bool值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Encrypt(bool value)
        {
            return Encrypt(value, 0);
        }
        /// <summary>
        /// 使用传递的密钥加密Bool值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int Encrypt(bool value, byte key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            var encryptedValue = value ? 213 : 181;

            encryptedValue ^= key;

            return encryptedValue;
        }

        /// <summary>
        /// 使用默认密钥解密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Decrypt(int value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// 使用传递的密钥解密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Decrypt(int value, byte key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            value ^= key;

            return value != 181;
        }

        public static TSBool FromEncrypted(int encrypted)
        {
            var instance = new TSBool();
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
            currentCryptoKey = (byte)ThreadSafeRandom.Next(150);
            hiddenValue = Encrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public int GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(int encrypted)
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
        public bool GetDecrypted()
        {
            return InternalDecrypt();
        }

        private bool InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(false);
                fakeValue = false;
                fakeValueActive = false;
                inited = true;

                return false;
            }

            var value = hiddenValue;
            value ^= currentCryptoKey;

            var decrypted = value != 181;

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && decrypted != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return decrypted;
        }

        #region overrides

        public static implicit operator TSBool(bool value)
        {
            return new TSBool(value);
        }

        public static implicit operator bool(TSBool value)
        {
            return value.InternalDecrypt();
        }

        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }
        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is TSBool))
                return false;
            return Equals((TSBool)obj);
        }

        public bool Equals(TSBool obj)
        {
            if (currentCryptoKey == obj.currentCryptoKey)
            {
                return hiddenValue == obj.hiddenValue;
            }

            return Decrypt(hiddenValue, currentCryptoKey) == Decrypt(obj.hiddenValue, obj.currentCryptoKey);
        }

        public int CompareTo(TSBool other)
        {
            return InternalDecrypt().CompareTo(other.InternalDecrypt());
        }

        public int CompareTo(bool other)
        {
            return InternalDecrypt().CompareTo(other);
        }

        public int CompareTo(object obj)
        {
#if !NO_IL2CPP
            return InternalDecrypt().CompareTo(obj);
#else
			if (obj == null) return 1;
			if (!(obj is bool)) throw new ArgumentException("Argument must be boolean");
			return CompareTo((bool)obj);
#endif
        }
        #endregion

    }
}

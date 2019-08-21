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
    public struct TSQuaternion
    {
        private static int cryptoKey = 120205;
        private static readonly Quaternion identity = Quaternion.identity;

        [SerializeField]
        private int currentCryptoKey;

        [SerializeField]
        private EncryptedQuaternion hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private Quaternion fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSQuaternion(Quaternion value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
			migratedVersion = null;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : identity;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        public TSQuaternion(float x, float y, float z, float w)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(x, y, z, w, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValue = new Quaternion(x, y, z, w);
                fakeValueActive = true;
            }
            else
            {
                fakeValue = identity;
                fakeValueActive = false;
            }

#if UNITY_EDITOR
			migratedVersion = null;
#endif

            inited = true;
        }

        /// <summary>
        /// 设置新的Key
        /// </summary>
        /// <param name="newKey"></param>
        public static void SetNewCryptoKey(int newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EncryptedQuaternion Encrypt(Quaternion value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static EncryptedQuaternion Encrypt(Quaternion value, int key)
        {
            return Encrypt(value.x, value.y, value.z, value.w, key);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        public static EncryptedQuaternion Encrypt(float x, float y, float z, float w, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            EncryptedQuaternion result;
            result.x = TSFloat.Encrypt(x, key);
            result.y = TSFloat.Encrypt(y, key);
            result.z = TSFloat.Encrypt(z, key);
            result.w = TSFloat.Encrypt(w, key);

            return result;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Quaternion Decrypt(EncryptedQuaternion value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Quaternion Decrypt(EncryptedQuaternion value, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            Quaternion result;
            result.x = TSFloat.Decrypt(value.x, key);
            result.y = TSFloat.Decrypt(value.y, key);
            result.z = TSFloat.Decrypt(value.z, key);
            result.w = TSFloat.Decrypt(value.w, key);

            return result;
        }

        public static TSQuaternion FromEncrypted(EncryptedQuaternion encrypted)
        {
            var instance = new TSQuaternion();
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
            currentCryptoKey = ThreadSafeRandom.Next();
            hiddenValue = Encrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// 获取加密后的值
        /// </summary>
        /// <returns></returns>
        public EncryptedQuaternion GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(EncryptedQuaternion encrypted)
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
        public Quaternion GetDecrypted()
        {
            return InternalDecrypt();
        }

        private Quaternion InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(identity);
                fakeValue = identity;
                fakeValueActive = false;
                inited = true;

                return identity;
            }

            Quaternion value;

            value.x = TSFloat.Decrypt(hiddenValue.x, currentCryptoKey);
            value.y = TSFloat.Decrypt(hiddenValue.y, currentCryptoKey);
            value.z = TSFloat.Decrypt(hiddenValue.z, currentCryptoKey);
            value.w = TSFloat.Decrypt(hiddenValue.w, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && !CompareQuaternionsWithTolerance(value, fakeValue))
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return value;
        }

        private bool CompareQuaternionsWithTolerance(Quaternion q1, Quaternion q2)
        {
            var epsilon = ObscuredCheatingDetector.Instance.QuaternionEpsilon;
            return Math.Abs(q1.x - q2.x) < epsilon &&
                   Math.Abs(q1.y - q2.y) < epsilon &&
                   Math.Abs(q1.z - q2.z) < epsilon &&
                   Math.Abs(q1.w - q2.w) < epsilon;
        }

#region overrides

        public static implicit operator TSQuaternion(Quaternion value)
        {
            return new TSQuaternion(value);
        }

        public static implicit operator Quaternion(TSQuaternion value)
        {
            return value.InternalDecrypt();
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }

        /// <summary>
        /// Returns a nicely formatted string of the Quaternion.
        /// </summary>
        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }

        /// <summary>
        /// Returns a nicely formatted string of the Quaternion.
        /// </summary>
        public string ToString(string format)
        {
            return InternalDecrypt().ToString(format);
        }

#endregion

        [Serializable]
        public struct EncryptedQuaternion
        {
            public int x;

            public int y;

            public int z;

            public int w;
        }
    }
}

#endif
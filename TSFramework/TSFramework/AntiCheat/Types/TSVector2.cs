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
    public struct TSVector2
    {
        private static int cryptoKey = 120206;
        private static readonly Vector2 zero = Vector2.zero;

        [SerializeField]
        private int currentCryptoKey;

        [SerializeField]
        private EncryptedVector2 hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private Vector2 fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSVector2(Vector2 value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
			migratedVersion = null;
#else
            var detectorRunning =  ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : zero;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// Mimics constructor of regular Vector2.
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        public TSVector2(float x, float y)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(x, y, currentCryptoKey);

            if ( ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValue = new Vector2(x, y);
                fakeValueActive = true;
            }
            else
            {
                fakeValue = zero;
                fakeValueActive = false;
            }

            inited = true;
        }

        public float x
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.x);
                if ( ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.x) >  ObscuredCheatingDetector.Instance.Vector2Epsilon)
                {
                     ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.x = InternalEncryptField(value);
                if ( ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.x = value;
                    fakeValue.y = InternalDecryptField(hiddenValue.y);
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public float y
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.y);
                if ( ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.y) >  ObscuredCheatingDetector.Instance.Vector2Epsilon)
                {
                     ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.y = InternalEncryptField(value);
                if ( ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.x = InternalDecryptField(hiddenValue.x);
                    fakeValue.y = value;
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException("Invalid TSVector2 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid TSVector2 index!");
                }
            }
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
        public static EncryptedVector2 Encrypt(Vector2 value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        public static EncryptedVector2 Encrypt(Vector2 value, int key)
        {
            return Encrypt(value.x, value.y, key);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        public static EncryptedVector2 Encrypt(float x, float y, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            EncryptedVector2 result;
            result.x = TSFloat.Encrypt(x, key);
            result.y = TSFloat.Encrypt(y, key);

            return result;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Vector2 Decrypt(EncryptedVector2 value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Vector2 Decrypt(EncryptedVector2 value, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            Vector2 result;
            result.x = TSFloat.Decrypt(value.x, key);
            result.y = TSFloat.Decrypt(value.y, key);

            return result;
        }

        public static TSVector2 FromEncrypted(EncryptedVector2 encrypted)
        {
            var instance = new TSVector2();
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
        public EncryptedVector2 GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(EncryptedVector2 encrypted)
        {
            inited = true;
            hiddenValue = encrypted;

            if (currentCryptoKey == 0)
            {
                currentCryptoKey = cryptoKey;
            }

            if ( ObscuredCheatingDetector.ExistsAndIsRunning)
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
        public Vector2 GetDecrypted()
        {
            return InternalDecrypt();
        }

        private Vector2 InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(zero);
                fakeValue = zero;
                fakeValueActive = false;
                inited = true;

                return zero;
            }

            Vector2 value;

            value.x = TSFloat.Decrypt(hiddenValue.x, currentCryptoKey);
            value.y = TSFloat.Decrypt(hiddenValue.y, currentCryptoKey);

            if ( ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && !CompareVectorsWithTolerance(value, fakeValue))
            {
                 ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return value;
        }

        private bool CompareVectorsWithTolerance(Vector2 vector1, Vector2 vector2)
        {
            var epsilon =  ObscuredCheatingDetector.Instance.Vector2Epsilon;
            return Math.Abs(vector1.x - vector2.x) < epsilon &&
                   Math.Abs(vector1.y - vector2.y) < epsilon;
        }

        private float InternalDecryptField(int encrypted)
        {
            var key = cryptoKey;

            if (currentCryptoKey != cryptoKey)
            {
                key = currentCryptoKey;
            }

            var result = TSFloat.Decrypt(encrypted, key);
            return result;
        }

        private int InternalEncryptField(float encrypted)
        {
            var result = TSFloat.Encrypt(encrypted, cryptoKey);
            return result;
        }

        #region overrides

        public static implicit operator TSVector2(Vector2 value)
        {
            return new TSVector2(value);
        }

        public static implicit operator Vector2(TSVector2 value)
        {
            return value.InternalDecrypt();
        }

        public static implicit operator Vector3(TSVector2 value)
        {
            var v = value.InternalDecrypt();
            return new Vector3(v.x, v.y, 0.0f);
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

        #endregion

        [Serializable]
        public struct EncryptedVector2
        {

            public int x;

            public int y;
        }
    }
}

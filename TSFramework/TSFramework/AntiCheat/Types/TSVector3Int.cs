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
    public struct TSVector3Int
    {
        private static int cryptoKey = 120207;
        private static readonly Vector3Int zero = Vector3Int.zero;

#if UNITY_EDITOR
		// For internal Editor usage only (may be useful for drawers).
		public static int cryptoKeyEditor = cryptoKey;
#endif

        [SerializeField]
        private int currentCryptoKey;

        [SerializeField]
        private EncryptedVector3Int hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private Vector3Int fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private TSVector3Int(Vector3Int value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

#if UNITY_EDITOR
			fakeValue = value;
			fakeValueActive = true;
#else
            var detectorRunning = ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : zero;
            fakeValueActive = detectorRunning;
#endif

            inited = true;
        }

        /// <summary>
        /// Mimics constructor of regular Vector3Int.
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        /// <param name="z">Z component of the vector</param>
        public TSVector3Int(int x, int y, int z)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(x, y, z, currentCryptoKey);

            if (ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValue = new Vector3Int
                {
                    x = x,
                    y = y,
                    z = z
                };
                fakeValueActive = true;
            }
            else
            {
                fakeValue = zero;
                fakeValueActive = false;
            }

            inited = true;
        }

        public int x
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.x);
                if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.x) > 0)
                {
                    ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.x = InternalEncryptField(value);
                if (ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.x = value;
                    fakeValue.y = InternalDecryptField(hiddenValue.y);
                    fakeValue.z = InternalDecryptField(hiddenValue.z);
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public int y
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.y);
                if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.y) > 0)
                {
                    ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.y = InternalEncryptField(value);
                if (ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.x = InternalDecryptField(hiddenValue.x);
                    fakeValue.y = value;
                    fakeValue.z = InternalDecryptField(hiddenValue.z);
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public int z
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.z);
                if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.z) > 0)
                {
                    ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.z = InternalEncryptField(value);
                if (ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.x = InternalDecryptField(hiddenValue.x);
                    fakeValue.y = InternalDecryptField(hiddenValue.y);
                    fakeValue.z = value;
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid TSVector3Int index!");
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
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid TSVector3Int index!");
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
        public static EncryptedVector3Int Encrypt(Vector3Int value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        public static EncryptedVector3Int Encrypt(Vector3Int value, int key)
        {
            return Encrypt(value.x, value.y, value.z, key);
        }

        /// <summary>
        /// 简单的对称加密，使用默认的加密密钥。
        /// </summary>
        public static EncryptedVector3Int Encrypt(int x, int y, int z, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            EncryptedVector3Int result;
            result.x = TSInt.Encrypt(x, key);
            result.y = TSInt.Encrypt(y, key);
            result.z = TSInt.Encrypt(z, key);

            return result;
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Vector3Int Decrypt(EncryptedVector3Int value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// 解密
        /// </summary>
        public static Vector3Int Decrypt(EncryptedVector3Int value, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            var result = new Vector3Int
            {
                x = TSInt.Decrypt(value.x, key),
                y = TSInt.Decrypt(value.y, key),
                z = TSInt.Decrypt(value.z, key)
            };

            return result;
        }

        public static TSVector3Int FromEncrypted(EncryptedVector3Int encrypted)
        {
            var instance = new TSVector3Int();
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
        public EncryptedVector3Int GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// 设置加密后的值
        /// </summary>
        /// <param name="encrypted"></param>
        public void SetEncrypted(EncryptedVector3Int encrypted)
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
        public Vector3Int GetDecrypted()
        {
            return InternalDecrypt();
        }

        private Vector3Int InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(zero, cryptoKey);
                fakeValue = zero;
                fakeValueActive = false;
                inited = true;

                return zero;
            }

            var value = new Vector3Int
            {
                x = TSInt.Decrypt(hiddenValue.x, currentCryptoKey),
                y = TSInt.Decrypt(hiddenValue.y, currentCryptoKey),
                z = TSInt.Decrypt(hiddenValue.z, currentCryptoKey)
            };


            if (ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && value != fakeValue)
            {
                ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return value;
        }

        private int InternalDecryptField(int encrypted)
        {
            var key = cryptoKey;

            if (currentCryptoKey != cryptoKey)
            {
                key = currentCryptoKey;
            }

            var result = TSInt.Decrypt(encrypted, key);
            return result;
        }

        private int InternalEncryptField(int encrypted)
        {
            var result = TSInt.Encrypt(encrypted, cryptoKey);
            return result;
        }

        #region overrides

        public static implicit operator TSVector3Int(Vector3Int value)
        {
            return new TSVector3Int(value);
        }

        public static implicit operator Vector3Int(TSVector3Int value)
        {
            return value.InternalDecrypt();
        }

        public static implicit operator Vector3(TSVector3Int value)
        {
            return value.InternalDecrypt();
        }

        public static TSVector3Int operator +(TSVector3Int a, TSVector3Int b)
        {
            return a.InternalDecrypt() + b.InternalDecrypt();
        }

        public static TSVector3Int operator +(Vector3Int a, TSVector3Int b)
        {
            return a + b.InternalDecrypt();
        }

        public static TSVector3Int operator +(TSVector3Int a, Vector3Int b)
        {
            return a.InternalDecrypt() + b;
        }

        public static TSVector3Int operator -(TSVector3Int a, TSVector3Int b)
        {
            return a.InternalDecrypt() - b.InternalDecrypt();
        }

        public static TSVector3Int operator -(Vector3Int a, TSVector3Int b)
        {
            return a - b.InternalDecrypt();
        }

        public static TSVector3Int operator -(TSVector3Int a, Vector3Int b)
        {
            return a.InternalDecrypt() - b;
        }

        public static TSVector3Int operator *(TSVector3Int a, int d)
        {
            return a.InternalDecrypt() * d;
        }

        public static bool operator ==(TSVector3Int lhs, TSVector3Int rhs)
        {
            return lhs.InternalDecrypt() == rhs.InternalDecrypt();
        }

        public static bool operator ==(Vector3Int lhs, TSVector3Int rhs)
        {
            return lhs == rhs.InternalDecrypt();
        }

        public static bool operator ==(TSVector3Int lhs, Vector3Int rhs)
        {
            return lhs.InternalDecrypt() == rhs;
        }

        public static bool operator !=(TSVector3Int lhs, TSVector3Int rhs)
        {
            return lhs.InternalDecrypt() != rhs.InternalDecrypt();
        }

        public static bool operator !=(Vector3Int lhs, TSVector3Int rhs)
        {
            return lhs != rhs.InternalDecrypt();
        }

        public static bool operator !=(TSVector3Int lhs, Vector3Int rhs)
        {
            return lhs.InternalDecrypt() != rhs;
        }

        public override bool Equals(object other)
        {
            return InternalDecrypt().Equals(other);
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
        public struct EncryptedVector3Int
        {
            public int x;

            public int y;

            public int z;
        }
    }
}

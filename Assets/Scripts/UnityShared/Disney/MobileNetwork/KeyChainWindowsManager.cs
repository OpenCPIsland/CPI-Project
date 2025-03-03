using LitJson;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;
using Disney.Mix.SDK;
using UnityEngine;
using System.Text;

namespace Disney.MobileNetwork
{
    public class KeyChainWindowsManager : KeyChainManager
    {


#if UNITY_STANDALONE_WIN

        private const string APP_DATA_KEY = "cp.AppData";

        private readonly IKeychain keychain;

        private const int InitializationVectorSize = 16;

        private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

        private static readonly byte[] tempInitializationVector = new byte[16];

        private readonly AesManaged symmetricAlgorithm;

        private KeyChainManager keyChainManager;


        public static byte[] getChainKey;

        private static byte[] localStorageKey = new byte[32];

        public KeyChainWindowsManager()
        {
        }

        //      [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
        //private static extern void _cryptProtectData(string dataIn, ref int dataOutSize, out IntPtr dataOut);

        //  [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
        // private static extern string _cryptUnprotectData(byte[] dataIn, int dataInLength);


        public byte[] Decrypt2(byte[] bytes)
        {
            string text = null;

            byte[] key = null;
            text = keyChainManager.GetString("SessionUnlockKey");// Convert.ToBase64String(getChainKey);

            if (string.IsNullOrEmpty(text))
            {
                key = new byte[32];
                getChainKey = key;
                key = getChainKey;
            }
            else
            {
                key = Convert.FromBase64String(text);
            }

            if (bytes.Length <= 16)
				{
					throw new ArgumentException("Invalid byte array: " + BitConverter.ToString(bytes) + ". Must be over 16 bytes long.");
				}
				if (key.Length != 32)
				{
					throw new ArgumentException("Invalid key: " + BitConverter.ToString(key) + ". Must be 32 bytes long.");
				}
				symmetricAlgorithm.Key = key;
				Array.Copy(bytes, 0, tempInitializationVector, 0, 16);
				symmetricAlgorithm.IV = tempInitializationVector;
				ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
				return cryptoTransform.TransformFinalBlock(bytes, 16, bytes.Length - 16);
        }

        public string Decrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            byte[] array = Convert.FromBase64String(text);
            byte[] array2 = new byte[32];
            byte[] array3 = new byte[array.Length - 32];
            Array.Copy(array, array2, 32);
            Array.ConstrainedCopy(array, 32, array3, 0, array.Length - 32);
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, array2);
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateDecryptor(bytes, bytes2))
                {
                    using (MemoryStream stream = new MemoryStream(array3))
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(stream2))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }


        public byte[] Encrypt2(byte[] bytes)
        {
            string text = null;

            byte[] key = null;

            text = getAppData()["SessionUnlockKey"];
            Debug.Log("Keys3: "+text);
            byte[] unused = Convert.FromBase64String(text);
            if (string.IsNullOrEmpty(text))
            {
                key = new byte[32];
                getChainKey = key;
                key = getChainKey;
            }
            else
            {
                key = Convert.FromBase64String(text);
            }

            if (unused.Length != 32)
			{
				throw new ArgumentException("Invalid key: " + BitConverter.ToString(unused) + ". Must be 32 bytes long.");
			}
			symmetricAlgorithm.Key = Convert.FromBase64String(getAppData()["SessionUnlockKey"]);
			rng.GetBytes(tempInitializationVector);
			symmetricAlgorithm.IV = tempInitializationVector;
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			byte[] array2 = new byte[16 + array.Length];
			Array.Copy(tempInitializationVector, 0, array2, 0, 16);
			Array.Copy(array, 0, array2, 16, array.Length);
			return array2;
        }
        public string Encrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 32);
            byte[] array = rfc2898DeriveBytes.Salt;
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateEncryptor(bytes, bytes2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(stream))
                            {
                                streamWriter.Write(text);
                            }
                        }
                        byte[] array2 = memoryStream.ToArray();
                        Array.Resize(ref array, array.Length + array2.Length);
                        Array.Copy(array2, 0, array, 32, array2.Length);
                        return Convert.ToBase64String(array);
                    }
                }
            }
        }

        private Dictionary<string, string> getAppData()
        {
            Dictionary<string, string> strs;
            string str = PlayerPrefs.GetString("cp.AppData", null);
            //string key = "";
            if (string.IsNullOrEmpty(str))
            {
                strs = new Dictionary<string, string>();
            }
            else
            {
                // Debug.Log(str);

                // Debug.Log("Keys0: " + Convert.ToBase64String(keychain.LocalStorageKey));
                //  byte[] numArray = Convert.FromBase64String(str);
                // string str1 = KeyChainWindowsManager._cryptUnprotectData(numArray, (int)numArray.Length);

                //string json = JsonMapper.ToJson(data);
                //string hexkey = BitConverter.ToString(getChainKey).Replace("-", "");
                // string bitString = BitConverter.ToString(Decrypt2(numArray));
                string str2 = Decrypt(str, "4C906C6AAF5C2CB4B581411A91091A8D");
                strs = JsonMapper.ToObject<Dictionary<string, string>>(str2);

              //  Debug.Log("Keys4: "+strs);

            }
            return strs;
        }


        public override string GetString(string key)
        {
            string str;
            this.appData = this.getAppData();
            this.appData.TryGetValue(key, out str);
            return str;
        }

        protected override void Init()
        {
            this.appData = this.getAppData();
        }

        public override void PutString(string key, string value)
        {
            this.appData[key] = value;
            this.setAppData(this.appData);
        }

        public override void RemoveString(string key)
        {
            if (this.appData.ContainsKey(key))
            {
                this.appData.Remove(key);
                this.setAppData(this.appData);
            }
        }

        private void setAppData(Dictionary<string, string> data)
        {
            // IntPtr intPtr;
            if (data == null)
            {
                PlayerPrefs.DeleteKey("cp.AppData");
            }
            string json = JsonMapper.ToJson(data);
            //Debug.Log("Keys: " + data["SessionUnlockKey"]);

             //byte[] jsonbytes = Encoding.ASCII.GetBytes(json);

             //byte[] keybytes = Convert.FromBase64String(data["SessionUnlockKey"]);

           //  getChainKey = keybytes;

         //  string hexkey =  BitConverter.ToString(keybytes).Replace("-", "");

           // Debug.Log("Keys1: " + hexkey);
            //int num = 0;
            //KeyChainWindowsManager._cryptProtectData(json, ref num, out intPtr);
            //byte[] numArray = new byte[num];
            //  Marshal.Copy(intPtr, numArray, 0, num);
            //  Marshal.FreeCoTaskMem(intPtr);

           // string bitString = BitConverter.ToString(Encrypt2(jsonbytes));
            PlayerPrefs.SetString("cp.AppData", Encrypt(json, "4C906C6AAF5C2CB4B581411A91091A8D"));

            //Encrypt(json, "4C906C6AAF5C2CB4B581411A91091A8D")
        }
        /*
#if UNITY_STANDALONE_WIN64 || UNITY_EDITOR_64
        private const string APP_DATA_KEY = "cp.AppData";

        public KeyChainWindowsManager()
        {
        }

        [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
        private static extern void _cryptProtectData(string dataIn, ref int dataOutSize, out IntPtr dataOut);

      //  [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
       // private static extern string _cryptUnprotectData(byte[] dataIn, int dataInLength);

        public string Decrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            byte[] array = Convert.FromBase64String(text);
            byte[] array2 = new byte[32];
            byte[] array3 = new byte[array.Length - 32];
            Array.Copy(array, array2, 32);
            Array.ConstrainedCopy(array, 32, array3, 0, array.Length - 32);
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, array2);
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateDecryptor(bytes, bytes2))
                {
                    using (MemoryStream stream = new MemoryStream(array3))
                    {
                        using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader(stream2))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }

        public string Encrypt(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 32);
            byte[] array = rfc2898DeriveBytes.Salt;
            byte[] bytes = rfc2898DeriveBytes.GetBytes(32);
            byte[] bytes2 = rfc2898DeriveBytes.GetBytes(16);
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Mode = CipherMode.CBC;
                aesManaged.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform transform = aesManaged.CreateEncryptor(bytes, bytes2))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(stream))
                            {
                                streamWriter.Write(text);
                            }
                        }
                        byte[] array2 = memoryStream.ToArray();
                        Array.Resize(ref array, array.Length + array2.Length);
                        Array.Copy(array2, 0, array, 32, array2.Length);
                        return Convert.ToBase64String(array);
                    }
                }
            }
        }

        private Dictionary<string, string> getAppData()
        {
            Dictionary<string, string> strs;
            string str = PlayerPrefs.GetString("cp.AppData", null);
            if (string.IsNullOrEmpty(str))
            {
                strs = new Dictionary<string, string>();
            }
            else
            {
            //    byte[] numArray = Convert.FromBase64String(str);
               // string str1 = KeyChainWindowsManager._cryptUnprotectData(numArray, (int)numArray.Length);
                string str2 = Decrypt(str, "4C906C6AAF5C2CB4B581411A91091A8D");
                strs = JsonMapper.ToObject<Dictionary<string, string>>(str2);
            }
            return strs;
        }

        public override string GetString(string key)
        {
            string str;
            this.appData = this.getAppData();
            this.appData.TryGetValue(key, out str);
            return str;
        }

        protected override void Init()
        {
            this.appData = this.getAppData();
        }

        public override void PutString(string key, string value)
        {
            this.appData[key] = value;
            this.setAppData(this.appData);
        }

        public override void RemoveString(string key)
        {
            if (this.appData.ContainsKey(key))
            {
                this.appData.Remove(key);
                this.setAppData(this.appData);
            }
        }

        private void setAppData(Dictionary<string, string> data)
        {
           // IntPtr intPtr;
            if (data == null)
            {
                PlayerPrefs.DeleteKey("cp.AppData");
            }
            string json = JsonMapper.ToJson(data);
            //int num = 0;
            //KeyChainWindowsManager._cryptProtectData(json, ref num, out intPtr);
            //byte[] numArray = new byte[num];
            //  Marshal.Copy(intPtr, numArray, 0, num);
            //  Marshal.FreeCoTaskMem(intPtr);
            PlayerPrefs.SetString("cp.AppData", Encrypt(json, "4C906C6AAF5C2CB4B581411A91091A8D"));
        }
#elif UNITY_STANDALONE_WIN32 || UNITY_EDITOR_32
       private const string APP_DATA_KEY = "cp.AppData";

        public KeyChainWindowsManager()
        {
        }

        [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
        private static extern void _cryptProtectData(string dataIn, ref int dataOutSize, out IntPtr dataOut);

        [DllImport("KeyChainWindows", CharSet = CharSet.Ansi, ExactSpelling = false)]
        private static extern string _cryptUnprotectData(byte[] dataIn, int dataInLength);

        private Dictionary<string, string> getAppData()
        {
            Dictionary<string, string> strs;
            string str = PlayerPrefs.GetString("cp.AppData", null);
            if (string.IsNullOrEmpty(str))
            {
                strs = new Dictionary<string, string>();
            }
            else
            {
                byte[] numArray = Convert.FromBase64String(str);
                string str1 = KeyChainWindowsManager._cryptUnprotectData(numArray, (int)numArray.Length);
                strs = JsonMapper.ToObject<Dictionary<string, string>>(str1);
            }
            return strs;
        }

        public override string GetString(string key)
        {
            string str;
            this.appData = this.getAppData();
            this.appData.TryGetValue(key, out str);
            return str;
        }

        protected override void Init()
        {
            this.appData = this.getAppData();
        }

        public override void PutString(string key, string value)
        {
            this.appData[key] = value;
            this.setAppData(this.appData);
        }

        public override void RemoveString(string key)
        {
            if (this.appData.ContainsKey(key))
            {
                this.appData.Remove(key);
                this.setAppData(this.appData);
            }
        }

        private void setAppData(Dictionary<string, string> data)
        {
            IntPtr intPtr;
            if (data == null)
            {
                PlayerPrefs.DeleteKey("cp.AppData");
            }
            string json = JsonMapper.ToJson(data);
            int num = 0;
            KeyChainWindowsManager._cryptProtectData(json, ref num, out intPtr);
            byte[] numArray = new byte[num];
            Marshal.Copy(intPtr, numArray, 0, num);
            Marshal.FreeCoTaskMem(intPtr);
            PlayerPrefs.SetString("cp.AppData", Convert.ToBase64String(numArray));
        }
#endif
        */
#endif
    }
}
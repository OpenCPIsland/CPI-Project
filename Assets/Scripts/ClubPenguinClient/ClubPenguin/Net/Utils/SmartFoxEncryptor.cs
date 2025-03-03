using ClubPenguin.Net.Client;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClubPenguin.Net.Utils
{
	public class SmartFoxEncryptor
	{
		private const int InitializationVectorSize = 16;

		private static readonly string ENCRYPTED_PARAMETER = "ep";

		private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

		private static readonly byte[] tempInitializationVector = new byte[16];

		private readonly AesManaged symmetricAlgorithm;

		public SmartFoxEncryptor(byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Invalid key: " + BitConverter.ToString(key) + ". Must be 32 bytes long.");
			}
			symmetricAlgorithm = new AesManaged
			{
				Key = key,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			};
		}

		public void EncryptParameter(string parameterKey, Dictionary<string, SFSDataWrapper> parameters)
		{
			if (!parameters.ContainsKey(parameterKey))
			{
				throw new Exception("Failed to encrypt parameter '" + parameterKey + "'. Parameter not found.");
			}
			SFSDataWrapper sFSDataWrapper = parameters[parameterKey];
			if (sFSDataWrapper.Type != 8)
			{
				throw new Exception("Failed to encrypt parameter '" + parameterKey + "'. Parameter is not a UTF string.");
			}
			string s = sFSDataWrapper.Data.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			byte[] inArray = Encrypt(bytes);
			string str = Convert.ToBase64String(inArray);
			parameters[parameterKey] = SmartFoxGameServerClientShared.serialize(str);
			parameters.Add(ENCRYPTED_PARAMETER, SmartFoxGameServerClientShared.serialize(parameterKey));
		}

		public void DecryptParameters(ISFSObject parameters)
		{
			if (parameters.ContainsKey(ENCRYPTED_PARAMETER))
			{
				string utfString = parameters.GetUtfString(ENCRYPTED_PARAMETER);
				string utfString2 = parameters.GetUtfString(utfString);
				byte[] bytes = Convert.FromBase64String(utfString2);
				byte[] bytes2 = Decrypt(bytes);
				string @string = Encoding.UTF8.GetString(bytes2);
				parameters.PutUtfString(utfString, @string);
			}
		}

		public byte[] Encrypt(byte[] bytes)
		{
			rng.GetBytes(tempInitializationVector);
			symmetricAlgorithm.IV = tempInitializationVector;
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			byte[] array2 = new byte[16 + array.Length];
			Array.Copy(tempInitializationVector, 0, array2, 0, 16);
			Array.Copy(array, 0, array2, 16, array.Length);
			return array2;
		}

		public byte[] Decrypt(byte[] bytes)
		{
			if (bytes.Length <= 16)
			{
				throw new ArgumentException("Invalid byte array: " + BitConverter.ToString(bytes) + ". Must be over 16 bytes long.");
			}
			Array.Copy(bytes, 0, tempInitializationVector, 0, 16);
			symmetricAlgorithm.IV = tempInitializationVector;
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
			return cryptoTransform.TransformFinalBlock(bytes, 16, bytes.Length - 16);
		}
	}
}

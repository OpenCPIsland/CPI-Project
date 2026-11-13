using System;
using System.Security.Cryptography;
using UnityEngine;

namespace Disney.Mix.SDK.Internal
{
	public class MixEncryptor : IEncryptor
	{
		private const int InitializationVectorSize = 16;

		private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

		private static readonly byte[] tempInitializationVector = new byte[16];

		private readonly AesManaged symmetricAlgorithm;

		public MixEncryptor()
		{
			symmetricAlgorithm = new AesManaged
			{
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7
			};
		}

		public byte[] Encrypt(byte[] bytes, byte[] key)
		{
			if (key.Length != 32)
			{
				throw new ArgumentException("Invalid key: " + BitConverter.ToString(key) + ". Must be 32 bytes long.");
			}

            Debug.Log("Original Key: " + Convert.ToBase64String(key));
            symmetricAlgorithm.Key = key;
			rng.GetBytes(tempInitializationVector);
			symmetricAlgorithm.IV = tempInitializationVector;
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
			byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			byte[] array2 = new byte[16 + array.Length];
			Array.Copy(tempInitializationVector, 0, array2, 0, 16);
			Array.Copy(array, 0, array2, 16, array.Length);
			return array2;
		}

		public byte[] Decrypt(byte[] bytes, byte[] key)
		{
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
	}
}

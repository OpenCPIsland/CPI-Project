using System;
using System.Security.Cryptography;

namespace DeviceDB
{
	internal class Aes256Encryptor
	{
		public const int KeySize = 32;

		public const int InitializationVectorSize = 16;

		public const uint RoundingMultiple = 16u;

		private readonly AesManaged symmetricAlgorithm;

		public byte[] Key
		{
			get;
			private set;
		}

		public byte[] InitializationVector
		{
			get;
			private set;
		}

		public Aes256Encryptor(byte[] key, byte[] initializationVector)
		{
			if (key == null || key.Length != 32)
			{
				throw new ArgumentException("Key is not " + 32 + " bytes long");
			}
			if (initializationVector == null || initializationVector.Length != 16)
			{
				throw new ArgumentException("Initialization vector is not " + 16 + " bytes long");
			}
			Key = key;
			InitializationVector = initializationVector;
			symmetricAlgorithm = new AesManaged
			{
				KeySize = 256,
				Key = key,
				IV = initializationVector
			};
		}

		public byte[] Encrypt(byte[] bytes)
		{
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateEncryptor();
			return cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
		}

		public byte[] Decrypt(byte[] bytes)
		{
			ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor();
			try
			{
				return cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			}
			catch (Exception innerException)
			{
				throw new CorruptionException("Failed to decrypt", innerException);
			}
		}
	}
}

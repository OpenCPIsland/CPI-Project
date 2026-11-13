using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Disney.LaunchPad.Packages.Cryptography
{
	public class ManagedAesCipher : ICipherStrategy
	{
		private const int m_saltSize = 32;

		private byte[] m_salt = new byte[32];

		private int m_iterations;

		private string m_password;

		private byte[] m_keyBytes = new byte[32];

		public ManagedAesCipher(string password, byte[] salt, int iterations)
		{
			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("A password must be provided!");
			}
			if (salt.Length != 32)
			{
				throw new ArgumentNullException("Salt must be 32 bytes long, invalid length provided!");
			}
			if (iterations <= 0)
			{
				throw new ArgumentNullException("iterations must be a positive integer!");
			}
			m_password = password;
			m_iterations = iterations;
			Array.Copy(salt, m_salt, 32);
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations);
			m_keyBytes = rfc2898DeriveBytes.GetBytes(32);
		}

		public byte[] Encrypt(string unencryptedText)
		{
			if (string.IsNullOrEmpty(unencryptedText))
			{
				throw new ArgumentNullException("unencryptedText");
			}
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(m_password, m_salt, m_iterations);
			byte[] bytes = rfc2898DeriveBytes.GetBytes(16);
			byte[] bytes2 = Encoding.Default.GetBytes(unencryptedText);
			using (AesManaged aesManaged = new AesManaged())
			{
				aesManaged.BlockSize = 128;
				aesManaged.KeySize = 128;
				aesManaged.Mode = CipherMode.CBC;
				aesManaged.Padding = PaddingMode.PKCS7;
				ICryptoTransform transform = aesManaged.CreateEncryptor(m_keyBytes, bytes);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						cryptoStream.Write(bytes2, 0, bytes2.Length);
						cryptoStream.FlushFinalBlock();
						byte[] array = memoryStream.ToArray();
						byte[] array2 = new byte[bytes.Length + memoryStream.Length];
						Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
						Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
						return array2;
					}
				}
			}
		}

		public string Decrypt(byte[] encryptedData)
		{
			if (encryptedData == null || encryptedData.Length <= 0)
			{
				throw new ArgumentNullException("encryptedData");
			}
			byte[] array = new byte[16];
			Buffer.BlockCopy(encryptedData, 0, array, 0, array.Length);
			byte[] array2 = new byte[encryptedData.Length - array.Length];
			Buffer.BlockCopy(encryptedData, array.Length, array2, 0, encryptedData.Length - array.Length);
			using (AesManaged aesManaged = new AesManaged())
			{
				aesManaged.BlockSize = 128;
				aesManaged.KeySize = 128;
				aesManaged.Mode = CipherMode.CBC;
				aesManaged.Padding = PaddingMode.PKCS7;
				ICryptoTransform transform = aesManaged.CreateDecryptor(m_keyBytes, array);
				using (MemoryStream stream = new MemoryStream(array2))
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
}

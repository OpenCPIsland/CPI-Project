using System;
using System.IO;
using System.Security.Cryptography;

namespace Disney.MobileNetwork
{
	public static class AesCipher
	{
		private const int SaltSize = 32;

		public static string Encrypt(string text, string key)
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

		public static string Decrypt(string text, string key)
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
	}
}

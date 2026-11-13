using System;
using System.Security.Cryptography;
using System.Text;

namespace Disney.MobileNetwork
{
	public class EncryptionHelper
	{
		private const string ENCRYPT_FILE_PREFIX = "__shhh__";

		private static MD5 s_hasher = null;

		public static string EncryptFile(string filename, string fileContent)
		{
			string text = Guid.NewGuid().ToString();
			string result = (fileContent == null || fileContent.Length == 0) ? fileContent : AesCipher.Encrypt(fileContent, text);
			EncryptedPlayerPrefs.SetEncryptedString("__shhh__" + filename, text);
			EncryptedPlayerPrefs.Save();
			return result;
		}

		public static string DecryptFile(string filename, string encryptedContent)
		{
			if (encryptedContent == null || encryptedContent.Length == 0)
			{
				return null;
			}
			string encryptedString = EncryptedPlayerPrefs.GetEncryptedString("__shhh__" + filename);
			if (encryptedString != null && encryptedString.Length > 0)
			{
				return AesCipher.Decrypt(encryptedContent, encryptedString);
			}
			return null;
		}

		public static string GetFileEncryptionKey(string filename)
		{
			return EncryptedPlayerPrefs.GetEncryptedString("__shhh__" + filename);
		}

		public static string GenerateHash(string srcString)
		{
			if (s_hasher == null)
			{
				s_hasher = MD5.Create();
			}
			byte[] bytes = s_hasher.ComputeHash(Encoding.UTF8.GetBytes(srcString));
			return StringHelper.ToHexString(bytes);
		}
	}
}

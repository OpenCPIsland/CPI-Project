using System;
using System.Security.Cryptography;
using System.Text;

namespace DI.HTTP.Security
{
	public class DigestHelper
	{
		private DigestHelper()
		{
		}

		public static string sha256(byte[] data)
		{
			try
			{
				SHA256 sHA = SHA256.Create();
				return byte2hex(sHA.ComputeHash(data));
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static string sha1(byte[] data)
		{
			try
			{
				SHA1 sHA = SHA1.Create();
				return byte2hex(sHA.ComputeHash(data));
			}
			catch (Exception)
			{
			}
			return null;
		}

		public static string byte2hex(byte[] data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				stringBuilder.Append(string.Format("{0:X2}", data[i]));
			}
			return stringBuilder.ToString();
		}
	}
}

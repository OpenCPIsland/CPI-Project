using System;
using System.Security.Cryptography;
using System.Text;

namespace ClubPenguin.Analytics
{
	public static class StringHasher
	{
		private static readonly HashAlgorithm hashAlgorithm = SHA256.Create();

		private static readonly Encoding stringEncoding = Encoding.UTF8;

		public static string Hash(string str)
		{
			byte[] bytes = stringEncoding.GetBytes(str);
			byte[] inArray = hashAlgorithm.ComputeHash(bytes);
			return Convert.ToBase64String(inArray);
		}

		public static string HashRounds(string str, int rounds)
		{
			for (int i = 0; i < rounds; i++)
			{
				str = Hash(str);
			}
			return str;
		}
	}
}

using System.Security.Cryptography;

namespace DeviceDB
{
	internal static class CryptoRandomNumberGenerator
	{
		private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

		public static byte[] GenerateBytes(uint length)
		{
			byte[] array = new byte[length];
			rng.GetBytes(array);
			return array;
		}
	}
}

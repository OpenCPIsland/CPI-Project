using System.Security.Cryptography;

namespace ClubPenguin.Net.Utils
{
	public static class RsaEncryptor
	{
		public static RSAParameters GenerateKeypair()
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				return rSACryptoServiceProvider.ExportParameters(true);
			}
		}

		public static byte[] Encrypt(byte[] plaintext, RSAParameters publicKey)
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rSACryptoServiceProvider.ImportParameters(publicKey);
				return rSACryptoServiceProvider.Encrypt(plaintext, false);
			}
		}

		public static byte[] Decrypt(byte[] ciphertext, RSAParameters privateKey)
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rSACryptoServiceProvider.ImportParameters(privateKey);
				return rSACryptoServiceProvider.Decrypt(ciphertext, false);
			}
		}
	}
}

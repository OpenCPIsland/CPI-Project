using System.Security.Cryptography;

namespace Disney.Mix.SDK.Internal
{
	public class RsaEncryptor : IRsaEncryptor
	{
		public RSAParameters GenerateKeypair()
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				return rSACryptoServiceProvider.ExportParameters(true);
			}
		}

		public byte[] Encrypt(byte[] plaintext, RSAParameters publicKey)
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rSACryptoServiceProvider.ImportParameters(publicKey);
				return rSACryptoServiceProvider.Encrypt(plaintext, false);
			}
		}

		public byte[] Decrypt(byte[] ciphertext, RSAParameters privateKey)
		{
			using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
			{
				rSACryptoServiceProvider.ImportParameters(privateKey);
				return rSACryptoServiceProvider.Decrypt(ciphertext, false);
			}
		}
	}
}

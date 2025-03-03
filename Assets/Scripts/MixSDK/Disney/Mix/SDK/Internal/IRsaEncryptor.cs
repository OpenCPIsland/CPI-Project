using System.Security.Cryptography;

namespace Disney.Mix.SDK.Internal
{
	public interface IRsaEncryptor
	{
		RSAParameters GenerateKeypair();

		byte[] Encrypt(byte[] plaintext, RSAParameters publicKey);

		byte[] Decrypt(byte[] ciphertext, RSAParameters privateKey);
	}
}

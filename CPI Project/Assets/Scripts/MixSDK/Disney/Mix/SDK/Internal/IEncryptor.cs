namespace Disney.Mix.SDK.Internal
{
	public interface IEncryptor
	{
		byte[] Encrypt(byte[] bytes, byte[] key);

		byte[] Decrypt(byte[] bytes, byte[] key);
	}
}

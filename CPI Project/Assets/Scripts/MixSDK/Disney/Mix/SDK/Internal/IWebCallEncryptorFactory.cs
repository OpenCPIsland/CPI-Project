namespace Disney.Mix.SDK.Internal
{
	public interface IWebCallEncryptorFactory
	{
		IWebCallEncryptor Create(byte[] key, long sessionId);
	}
}

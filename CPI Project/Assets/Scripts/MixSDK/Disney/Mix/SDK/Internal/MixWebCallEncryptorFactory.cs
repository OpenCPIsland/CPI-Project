namespace Disney.Mix.SDK.Internal
{
	public class MixWebCallEncryptorFactory : IWebCallEncryptorFactory
	{
		private readonly IEncryptor encryptor;

		public MixWebCallEncryptorFactory(IEncryptor encryptor)
		{
			this.encryptor = encryptor;
		}

		public IWebCallEncryptor Create(byte[] key, long sessionId)
		{
			return new MixWebCallEncryptor(key, sessionId, encryptor);
		}
	}
}

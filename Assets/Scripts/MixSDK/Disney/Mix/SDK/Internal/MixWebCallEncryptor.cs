using System;

namespace Disney.Mix.SDK.Internal
{
	public class MixWebCallEncryptor : IWebCallEncryptor
	{
		private readonly IEncryptor encryptor;

		private readonly byte[] encryptionKey;

		public string ContentType
		{
			get
			{
				return "application/mix";
			}
		}

		public string SessionId
		{
			get;
			private set;
		}

		public MixWebCallEncryptor(byte[] encryptionKey, long sessionId, IEncryptor encryptor)
		{
			if (encryptionKey.Length != 32)
			{
				throw new ArgumentException("Invalid key: " + BitConverter.ToString(encryptionKey) + ". Must be 32 bytes long.");
			}
			this.encryptor = encryptor;
			this.encryptionKey = encryptionKey;
			SessionId = sessionId.ToString();
		}

		public byte[] Encrypt(byte[] bytes)
		{
			return encryptor.Encrypt(bytes, encryptionKey);
		}

		public byte[] Decrypt(byte[] bytes)
		{
			return encryptor.Decrypt(bytes, encryptionKey);
		}
	}
}

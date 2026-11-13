namespace Disney.Mix.SDK.Internal
{
	public class MixSessionStartWebCallEncryptor : IWebCallEncryptor
	{
		public string ContentType
		{
			get
			{
				return "application/json";
			}
		}

		public string SessionId
		{
			get
			{
				return null;
			}
		}

		public byte[] Encrypt(byte[] bytes)
		{
			return bytes;
		}

		public byte[] Decrypt(byte[] bytes)
		{
			return bytes;
		}
	}
}

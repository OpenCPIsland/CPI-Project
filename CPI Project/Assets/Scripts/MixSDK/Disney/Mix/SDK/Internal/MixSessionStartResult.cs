namespace Disney.Mix.SDK.Internal
{
	public class MixSessionStartResult
	{
		public IWebCallEncryptor WebCallEncryptor
		{
			get;
			private set;
		}

		public MixSessionStartResult(IWebCallEncryptor webCallEncryptor)
		{
			WebCallEncryptor = webCallEncryptor;
		}
	}
}

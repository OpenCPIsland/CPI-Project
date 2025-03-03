namespace Disney.Mix.SDK.Internal
{
	public class LoadAssetResult
	{
		public bool Success
		{
			get;
			private set;
		}

		public byte[] Bytes
		{
			get;
			private set;
		}

		public LoadAssetResult(bool success, byte[] bytes)
		{
			Success = success;
			Bytes = bytes;
		}
	}
}

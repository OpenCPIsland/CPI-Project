namespace Disney.Kelowna.Common
{
	public class AsyncStreamingAssetBundleRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		public readonly StreamingAssetBundleWrapper StreamingAssetBundleRequest;

		public override TAsset Asset
		{
			get
			{
				return (StreamingAssetBundleRequest != null) ? ((TAsset)(object)StreamingAssetBundleRequest.AssetBundle) : null;
			}
		}

		public AsyncStreamingAssetBundleRequest(string key, StreamingAssetBundleWrapper request)
			: base(key, new CoroutineReturn[1]
			{
				request
			})
		{
			StreamingAssetBundleRequest = request;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && StreamingAssetBundleRequest != null)
				{
					StreamingAssetBundleRequest.Dispose();
				}
				disposed = true;
			}
		}
	}
}

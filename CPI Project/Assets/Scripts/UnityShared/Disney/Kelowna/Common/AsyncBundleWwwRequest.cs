namespace Disney.Kelowna.Common
{
	public class AsyncBundleWwwRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		public readonly AssetBundleWwwWrapper WwwRequest;

		public override TAsset Asset
		{
			get
			{
				return (WwwRequest != null) ? ((TAsset)(object)WwwRequest.AssetBundle) : null;
			}
		}

		public AsyncBundleWwwRequest(string key, AssetBundleWwwWrapper request)
			: base(key, new CoroutineReturn[1]
			{
				request
			})
		{
			WwwRequest = request;
		}
	}
}

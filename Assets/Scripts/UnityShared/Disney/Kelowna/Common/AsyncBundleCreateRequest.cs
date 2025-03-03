namespace Disney.Kelowna.Common
{
	public class AsyncBundleCreateRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		public readonly AssetBundleCreateWrapper CreateRequest;

		public override TAsset Asset
		{
			get
			{
				return (CreateRequest != null) ? ((TAsset)(object)CreateRequest.AssetBundle) : null;
			}
		}

		public AsyncBundleCreateRequest(string key, AssetBundleCreateWrapper request)
			: base(key, new CoroutineReturn[1]
			{
				request
			})
		{
			CreateRequest = request;
		}
	}
}

namespace Disney.Kelowna.Common
{
	public class AsyncAssetRequest<TAsset> : AssetRequest<TAsset> where TAsset : class
	{
		public AssetRequest<TAsset> Request
		{
			get
			{
				return (AssetRequest<TAsset>)coroutineReturns[0];
			}
			set
			{
				coroutineReturns[0] = value;
			}
		}

		public override TAsset Asset
		{
			get
			{
				return Request.Asset;
			}
		}

		public AsyncAssetRequest(string key, AssetRequestWrapper<TAsset> request)
			: base(key, new CoroutineReturn[1]
			{
				request
			})
		{
		}
	}
}

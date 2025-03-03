using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class AsyncAssetBundleRequest<TAsset> : AbstractAsyncAssetRequest<TAsset, AssetBundleRequest> where TAsset : class
	{
		public override TAsset Asset
		{
			get
			{
				return (base.Request != null) ? ((TAsset)(object)base.Request.asset) : null;
			}
		}

		public AsyncAssetBundleRequest(string key, AssetBundleRequest request)
			: base(key, request)
		{
		}
	}
}

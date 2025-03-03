using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class AsyncAssetResourceRequest<TAsset> : AbstractAsyncAssetRequest<TAsset, ResourceRequest> where TAsset : class
	{
		public override TAsset Asset
		{
			get
			{
				return (TAsset)(object)base.Request.asset;
			}
		}

		public AsyncAssetResourceRequest(string key, ResourceRequest request)
			: base(key, request)
		{
		}
	}
}

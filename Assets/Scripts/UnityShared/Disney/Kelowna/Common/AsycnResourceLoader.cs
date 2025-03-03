using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class AsycnResourceLoader<TAsset> where TAsset : class
	{
		public static AssetRequest<TAsset> Load(ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			ResourceRequest resourceRequest = Resources.LoadAsync(entry.Key, typeof(TAsset));
			if (resourceRequest.isDone && resourceRequest.asset == null)
			{
				throw new ArgumentException("Asset could not be loaded. Is the key correct? Key = " + entry.Key);
			}
			AsyncAssetResourceRequest<TAsset> asyncAssetResourceRequest = new AsyncAssetResourceRequest<TAsset>(entry.Key, resourceRequest);
			if (handler != null)
			{
				CoroutineRunner.StartPersistent(waitForLoadToFinish(entry.Key, asyncAssetResourceRequest, handler), typeof(AsycnResourceLoader<TAsset>), "waitForLoadToFinish");
			}
			return asyncAssetResourceRequest;
		}

		private static IEnumerator waitForLoadToFinish(string key, AssetRequest<TAsset> request, AssetLoadedHandler<TAsset> handler)
		{
			yield return request;
			handler(key, request.Asset);
		}
	}
}

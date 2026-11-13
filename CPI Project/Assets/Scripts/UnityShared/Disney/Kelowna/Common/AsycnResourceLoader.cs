using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class AsycnResourceLoader<TAsset> where TAsset : class
	{
		public static AssetRequest<TAsset> Load(ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			string resourcePath = ResourcePathResolver.Resolve(entry.Key, typeof(TAsset));
			ResourceRequest resourceRequest = Resources.LoadAsync(resourcePath, typeof(TAsset));
			if (resourceRequest.isDone && resourceRequest.asset == null)
			{
				throw new ArgumentException("Asset could not be loaded. Is the key correct? Key = " + entry.Key + ", resolved path = " + resourcePath);
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

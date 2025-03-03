using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class LocalWwwBundleDevice : Device
	{
		public const string DEVICE_TYPE = "local-www-bundle";

		private string baseUri;

		private IGcsAccessTokenService gcsAccessTokenService;

		public override string DeviceType
		{
			get
			{
				return "local-www-bundle";
			}
		}

		public LocalWwwBundleDevice(DeviceManager deviceManager, string baseUri)
			: this(deviceManager)
		{
			this.baseUri = baseUri;
		}

		public LocalWwwBundleDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			gcsAccessTokenService = Service.Get<IGcsAccessTokenService>();
			string bundlePath = UriUtil.Combine(baseUri, entry.Key);
			AssetBundleWwwWrapper assetBundleWwwWrapper = new AssetBundleWwwWrapper(bundlePath, gcsAccessTokenService);
			AsyncBundleWwwRequest<TAsset> result = new AsyncBundleWwwRequest<TAsset>(entry.Key, assetBundleWwwWrapper);
			CoroutineRunner.StartPersistent(waitForBundleToLoad(assetBundleWwwWrapper, handler, entry.IsCacheOnly), this, "Local_waitForBundleToLoad");
			return result;
		}

		private IEnumerator waitForBundleToLoad<TAsset>(AssetBundleWwwWrapper bundleRequest, AssetLoadedHandler<TAsset> handler, bool cacheOnly) where TAsset : class
		{
			bundleRequest.LoadFromDownload(bundleRequest.BundlePath);
			Service.Get<LoadingController>().RegisterDownload(bundleRequest.WebRequest);
			yield return bundleRequest.Send();
			Service.Get<LoadingController>().UnRegisterDownload(bundleRequest.WebRequest);
			if (handler != null)
			{
				AssetBundle assetBundle = null;
				if (!cacheOnly)
				{
					assetBundle = bundleRequest.AssetBundle;
				}
				TAsset asset = null;
				if (assetBundle != null)
				{
					asset = (TAsset)(object)assetBundle;
				}
				handler(bundleRequest.BundlePath, asset);
			}
			yield return null;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			throw new InvalidOperationException("Local asset bundles must be loaded asynchronously.");
		}
	}
}

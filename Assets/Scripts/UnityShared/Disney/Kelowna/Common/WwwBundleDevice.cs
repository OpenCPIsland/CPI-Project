using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class WwwBundleDevice : Device
	{
		public const string DEVICE_TYPE = "www-bundle";

		private string baseUri = "";

		private IGcsAccessTokenService gcsAccessTokenService;

		private ICPipeManifestService cpipeManifestService;

		private static DevCacheableType<bool> delayLoading = new DevCacheableType<bool>("content.delayBundleLoading", true);

		[Tweakable("Content.Delay Bundle Loading")]
		public static bool DelayLoading
		{
			get
			{
				return delayLoading.Value;
			}
			set
			{
				delayLoading.Value = value;
			}
		}

		public override string DeviceType
		{
			get
			{
				return "www-bundle";
			}
		}

		public WwwBundleDevice(DeviceManager deviceManager, string baseUri)
			: this(deviceManager, Service.Get<IGcsAccessTokenService>(), Service.Get<ICPipeManifestService>())
		{
			this.baseUri = baseUri;
		}

		public void UpdateBaseUri(string baseUri)
		{
			this.baseUri = baseUri;
		}

		public WwwBundleDevice(DeviceManager deviceManager, IGcsAccessTokenService gcsAccessTokenService, ICPipeManifestService cpipeManifestService)
			: base(deviceManager)
		{
			this.gcsAccessTokenService = gcsAccessTokenService;
			this.cpipeManifestService = cpipeManifestService;
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			string bundlePath = UriUtil.Combine(baseUri, entry.Key);
			AssetBundleWwwWrapper assetBundleWwwWrapper = new AssetBundleWwwWrapper(bundlePath, gcsAccessTokenService);
			AsyncBundleWwwRequest<TAsset> result = new AsyncBundleWwwRequest<TAsset>(entry.Key, assetBundleWwwWrapper);
			uint result2 = 0u;
			if (entry.UserData != null && entry.UserData is ContentManifest.BundleEntry)
			{
				ContentManifest.BundleEntry bundleEntry = (ContentManifest.BundleEntry)entry.UserData;
				if (!uint.TryParse(bundleEntry.Crc, out result2))
				{
					result2 = 0u;
				}
			}
			CoroutineRunner.StartPersistent(waitForBundleToLoad(assetBundleWwwWrapper, result2, handler), this, "waitForBundleToLoad");
			return result;
		}

		private IEnumerator waitForBundleToLoad<TAsset>(AssetBundleWwwWrapper bundleRequestWrapper, uint crc, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			CPipeManifestResponse cpipeManifestResponse = new CPipeManifestResponse();
			yield return cpipeManifestService.LookupAssetUrl(cpipeManifestResponse, bundleRequestWrapper.BundlePath);
			if (string.IsNullOrEmpty(cpipeManifestResponse.FullAssetUrl))
			{
				throw new Exception(string.Format("Bundle \"{0}\" NOT FOUND in CPipe manifest.", bundleRequestWrapper.BundlePath));
			}
			while (!Caching.ready)
			{
				yield return null;
			}
			bundleRequestWrapper.LoadFromCacheOrDownload(cpipeManifestResponse.FullAssetUrl, crc);
			Service.Get<LoadingController>().RegisterDownload(bundleRequestWrapper.WebRequest);
			yield return bundleRequestWrapper.Send();
			Service.Get<LoadingController>().UnRegisterDownload(bundleRequestWrapper.WebRequest);
			if (DelayLoading)
			{
				yield return null;
				yield return null;
			}
			for (int i = 0; i < 3; i++)
			{
				if (bundleRequestWrapper.WebRequest.isNetworkError)
				{
					Log.LogErrorFormatted(this, "Retry count {0}. Failed to download bundle {1} with error: {2}", i + 1, bundleRequestWrapper.BundlePath, bundleRequestWrapper.WebRequest.error);
				}
				else
				{
					if (!(bundleRequestWrapper.AssetBundle == null))
					{
						break;
					}
					Log.LogErrorFormatted(this, "Retry count {0}. Downloaded bundle was null", i + 1);
				}
				bundleRequestWrapper.LoadFromCacheOrDownload(cpipeManifestResponse.FullAssetUrl, crc);
				string message = string.Format("Retry bundle load with expected CRC {0}: {1}", crc, bundleRequestWrapper.BundlePath);
				Crittercism.LeaveBreadcrumb(message);
				Service.Get<LoadingController>().RegisterDownload(bundleRequestWrapper.WebRequest);
				yield return bundleRequestWrapper.Send();
				Service.Get<LoadingController>().UnRegisterDownload(bundleRequestWrapper.WebRequest);
			}
			if (bundleRequestWrapper.AssetBundle != null)
			{
				string breadcrumb = string.Format("Loaded bundle with expected CRC {0}: {1}", crc, bundleRequestWrapper.BundlePath);
				Crittercism.LeaveBreadcrumb(breadcrumb);
			}
			else
			{
				string breadcrumb = string.Format("Failed to load bundle with expected CRC {0}: {1}", crc, bundleRequestWrapper.BundlePath);
				Crittercism.LeaveBreadcrumb(breadcrumb);
			}
			if (handler != null)
			{
				TAsset asset = null;
				if (bundleRequestWrapper.AssetBundle != null)
				{
					asset = (TAsset)(object)bundleRequestWrapper.AssetBundle;
				}
				handler(bundleRequestWrapper.BundlePath, asset);
			}
			bundleRequestWrapper.IsComplete = true;
			bundleRequestWrapper.CacheAndDispose();
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			throw new InvalidOperationException("Remote asset bundles must be loaded asynchronously.");
		}
	}
}

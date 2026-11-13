using Disney.LaunchPadFramework;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class LoadBundleFromDiskDevice : Device
	{
		public const string DEVICE_TYPE = "load-bundle";

		public override string DeviceType
		{
			get
			{
				return "load-bundle";
			}
		}

		public LoadBundleFromDiskDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetBundleCreateWrapper assetBundleCreateWrapper = new AssetBundleCreateWrapper(null);
			AsyncBundleCreateRequest<TAsset> result = new AsyncBundleCreateRequest<TAsset>(entry.Key, assetBundleCreateWrapper);
			CoroutineRunner.StartPersistent(waitForBundleToCreate(entry.Key, assetBundleCreateWrapper, handler), this, "waitForBundleToCreate");
			return result;
		}

		private IEnumerator waitForBundleToCreate<TAsset>(string key, AssetBundleCreateWrapper bundleRequest, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			AssetBundleCreateRequest bundleLoadRequest = (AssetBundleCreateRequest)(bundleRequest.MutableOperation = AssetBundle.LoadFromFileAsync(key));
			yield return bundleLoadRequest;
			AssetBundle bundle = bundleLoadRequest.assetBundle;
			if (bundle == null)
			{
				Log.LogError(this, "Failed to load asset bundle:" + key);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			string bundleKey = entry.BundleKey;
			AssetBundle assetBundle = AssetBundle.LoadFromFile(bundleKey);
			Object @object = assetBundle.LoadAsset<Object>("dimpledsphere");
			return @object as TAsset;
		}
	}
}

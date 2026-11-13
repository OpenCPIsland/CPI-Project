using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Disney.Kelowna.Common
{
	public class BundleDevice : Device
	{
		public const string DEVICE_TYPE = "bundle";

		private readonly BundleManager bundleManager;

		public override string DeviceType
		{
			get
			{
				return "bundle";
			}
		}

		public BundleDevice(DeviceManager deviceManager, BundleManager bundleManager)
			: base(deviceManager)
		{
			this.bundleManager = bundleManager;
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			string entryKey = entry.Key;
			if (entry.Extension.Equals("unity"))
			{
				AsyncOperationHandle dependencyHandle = bundleManager.DownloadAddressableDependencies(entryKey, entry.BundleKey, entry.AssetPath.ToLowerInvariant());
				AddressableDependencyRequest<TAsset> dependencyRequest = new AddressableDependencyRequest<TAsset>(entryKey, dependencyHandle);
				if (handler != null)
				{
					dependencyHandle.Completed += operation => handler(entryKey, null);
				}
				return dependencyRequest;
			}
			AsyncOperationHandle<TAsset> handle = bundleManager.LoadAddressableAsync<TAsset>(entryKey, entry.BundleKey, entry.AssetPath.ToLowerInvariant());
			AddressableAssetRequest<TAsset> request = new AddressableAssetRequest<TAsset>(entryKey, handle);
			if (handler != null)
			{
				handle.Completed += operation => handler(entryKey, operation.Status == AsyncOperationStatus.Succeeded ? operation.Result : null);
			}
			return request;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			return bundleManager.LoadAddressableImmediate<TAsset>(entry.Key, entry.BundleKey, entry.AssetPath.ToLowerInvariant());
		}
	}
}

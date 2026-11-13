using System.Collections;

namespace Disney.Kelowna.Common
{
	public class AssetDatabaseDevice : Device
	{
		public const string DEVICE_TYPE = "asset-database";

		public override string DeviceType
		{
			get
			{
				return "asset-database";
			}
		}

		public AssetDatabaseDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForAssetToLoad(asyncAssetRequest, entry.Key, entry.AssetPath, handler), this, "waitForAssetToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForAssetToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, string key, string assetPath, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			yield return null;
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			return null;
		}
	}
}

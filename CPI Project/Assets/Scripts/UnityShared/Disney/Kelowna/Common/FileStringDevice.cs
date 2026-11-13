using System.Collections;
using System.IO;

namespace Disney.Kelowna.Common
{
	public class FileStringDevice : Device
	{
		public const string DEVICE_TYPE = "file-string";

		public override string DeviceType
		{
			get
			{
				return "file-string";
			}
		}

		public FileStringDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			string key = entry.Key + '.' + entry.Extension;
			CoroutineRunner.StartPersistent(waitForFileStreamAssetToLoad(asyncAssetRequest, key, handler), this, "waitForFileStreamAssetToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForFileStreamAssetToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			string fileText;
			using (StreamReader streamReader = new StreamReader(key))
			{
				fileText = streamReader.ReadToEnd();
			}
			assetRequest.Request = new IndexedAssetRequest<TAsset>(key, (TAsset)(object)fileText);
			yield return assetRequest;
			if (handler != null)
			{
				handler(key, (TAsset)(object)fileText);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			string text = File.ReadAllText(entry.AssetPath);
			return (TAsset)(object)text;
		}
	}
}

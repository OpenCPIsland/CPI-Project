using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class BytesDevice : Device
	{
		public const string DEVICE_TYPE = "bytes";

		public override string DeviceType
		{
			get
			{
				return "bytes";
			}
		}

		public BytesDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequest<TextAsset> textRequest = DeviceManager.LoadAsync<TextAsset>(deviceList, ref entry);
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForTextAssetToLoad(asyncAssetRequest, textRequest, entry.Key, handler), this, "waitForTextAssetToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForTextAssetToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, AssetRequest<TextAsset> textRequest, string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			yield return textRequest;
			byte[] bytes = textRequest.Asset.bytes;
			assetRequest.Request = new IndexedAssetRequest<TAsset>(key, (TAsset)(object)bytes);
			yield return assetRequest;
			if (handler != null)
			{
				handler(key, (TAsset)(object)bytes);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			TextAsset textAsset = DeviceManager.LoadImmediate<TextAsset>(deviceList, ref entry);
			return (TAsset)(object)textAsset.bytes;
		}
	}
}

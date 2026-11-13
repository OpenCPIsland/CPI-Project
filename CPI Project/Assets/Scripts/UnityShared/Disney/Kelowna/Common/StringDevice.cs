using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class StringDevice : Device
	{
		public const string DEVICE_TYPE = "str";

		public override string DeviceType
		{
			get
			{
				return "str";
			}
		}

		public StringDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequest<TextAsset> textRequest = DeviceManager.LoadAsync<TextAsset>(deviceList, ref entry);
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForStringToLoad(asyncAssetRequest, textRequest, entry.Key, handler), this, "waitForStringToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForStringToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, AssetRequest<TextAsset> textRequest, string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			yield return textRequest;
			string text = textRequest.Asset.text;
			assetRequest.Request = new IndexedAssetRequest<TAsset>(key, (TAsset)(object)text);
			yield return assetRequest;
			if (handler != null)
			{
				handler(key, (TAsset)(object)text);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			TextAsset textAsset = DeviceManager.LoadImmediate<TextAsset>(deviceList, ref entry);
			return (TAsset)(object)textAsset.text;
		}
	}
}

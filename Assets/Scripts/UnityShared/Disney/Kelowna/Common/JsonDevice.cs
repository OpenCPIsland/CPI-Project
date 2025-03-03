using Disney.MobileNetwork;
using System.Collections;

namespace Disney.Kelowna.Common
{
	public class JsonDevice : Device
	{
		public const string DEVICE_TYPE = "json";

		public override string DeviceType
		{
			get
			{
				return "json";
			}
		}

		public JsonDevice(DeviceManager deviceManager)
			: base(deviceManager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			AssetRequest<string> stringRequest = DeviceManager.LoadAsync<string>(deviceList, ref entry);
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForStringToLoad(asyncAssetRequest, stringRequest, entry.Key, handler), this, "waitForStringToLoad");
			return asyncAssetRequest;
		}

		private IEnumerator waitForStringToLoad<TAsset>(AsyncAssetRequest<TAsset> assetRequest, AssetRequest<string> stringRequest, string key, AssetLoadedHandler<TAsset> handler) where TAsset : class
		{
			yield return stringRequest;
			string jsonString = stringRequest.Asset;
			JsonService json = Service.Get<JsonService>();
			TAsset jsonAsset = json.Deserialize<TAsset>(jsonString);
			assetRequest.Request = new IndexedAssetRequest<TAsset>(key, jsonAsset);
			if (handler != null)
			{
				handler(key, jsonAsset);
			}
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			string stringToDeserialize = DeviceManager.LoadImmediate<string>(deviceList, ref entry);
			JsonService jsonService = Service.Get<JsonService>();
			return jsonService.Deserialize<TAsset>(stringToDeserialize);
		}
	}
}

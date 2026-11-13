using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class MockTextAssetDevice : Device
	{
		public override string DeviceType
		{
			get
			{
				return "mock-text-asset";
			}
		}

		public MockTextAssetDevice(DeviceManager manager)
			: base(manager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			string key = entry.Key;
			TextAsset textAsset = Resources.Load<TextAsset>(key);
			return new IndexedAssetRequest<TAsset>(entry.Key, (TAsset)(object)textAsset);
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			string key = entry.Key;
			TextAsset textAsset = Resources.Load<TextAsset>(key);
			return (TAsset)(object)textAsset;
		}
	}
}

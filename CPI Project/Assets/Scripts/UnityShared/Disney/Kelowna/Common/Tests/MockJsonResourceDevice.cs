using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.Tests
{
	public class MockJsonResourceDevice : Device
	{
		public override string DeviceType
		{
			get
			{
				return "mock-json-res";
			}
		}

		public MockJsonResourceDevice(DeviceManager manager)
			: base(manager)
		{
		}

		public override AssetRequest<TAsset> LoadAsync<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry, AssetLoadedHandler<TAsset> handler = null)
		{
			Resources.Load<TextAsset>(entry.Key);
			ResourceRequest unityRequest = Resources.LoadAsync<TextAsset>(entry.Key);
			AssetRequestWrapper<TAsset> request = new AssetRequestWrapper<TAsset>(null);
			AsyncAssetRequest<TAsset> asyncAssetRequest = new AsyncAssetRequest<TAsset>(entry.Key, request);
			CoroutineRunner.StartPersistent(waitForTextAssetToLoad(entry.Key, unityRequest, asyncAssetRequest), this, "test");
			return asyncAssetRequest;
		}

		private IEnumerator waitForTextAssetToLoad<TAsset>(string key, ResourceRequest unityRequest, AsyncAssetRequest<TAsset> request) where TAsset : class
		{
			yield return unityRequest;
			string text = (unityRequest.asset as TextAsset).text;
			request.Request = new IndexedAssetRequest<TAsset>(key, (TAsset)(object)text);
		}

		public override TAsset LoadImmediate<TAsset>(string deviceList, ref ContentManifest.AssetEntry entry)
		{
			TextAsset textAsset = Resources.Load<TextAsset>(entry.Key);
			return (TAsset)(object)textAsset.text;
		}
	}
}

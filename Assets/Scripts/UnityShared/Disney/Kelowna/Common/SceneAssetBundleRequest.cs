using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class SceneAssetBundleRequest<TAsset> : AsyncAssetBundleRequest<TAsset> where TAsset : class
	{
		public override bool Finished
		{
			get;
			set;
		}

		public string BundleKey
		{
			get;
			private set;
		}

		public SceneAssetBundleRequest(string sceneKey, string bundleKey, AssetBundleRequest request)
			: base(sceneKey, request)
		{
			BundleKey = bundleKey;
		}
	}
}

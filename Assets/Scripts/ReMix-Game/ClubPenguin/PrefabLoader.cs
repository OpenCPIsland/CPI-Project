using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class PrefabLoader : MonoBehaviour
	{
		public PrefabContentKey PrefabToLoad;

		private void Start()
		{
			Service.Get<LoadingController>().AddLoadingSystem(this);
			CoroutineRunner.Start(loadPrefab(), this, "loadPrefab");
		}

		private IEnumerator loadPrefab()
		{
			AssetRequest<GameObject> request = Content.LoadAsync<GameObject>(PrefabToLoad.Key);
			yield return request;
			if (request.Asset != null)
			{
				Object.Instantiate(request.Asset, base.transform);
			}
			else
			{
				Log.LogErrorFormatted(this, "Failed to load prefab for content key {0}", PrefabToLoad.Key);
			}
			LoadingController loadingController = Service.Get<LoadingController>();
			if (loadingController.HasLoadingSystem(this))
			{
				loadingController.RemoveLoadingSystem(this);
			}
		}
	}
}

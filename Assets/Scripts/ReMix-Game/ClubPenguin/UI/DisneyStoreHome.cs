using ClubPenguin.DisneyStore;
using Disney.Kelowna.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DisneyStoreHome : MonoBehaviour
	{
		[SerializeField]
		private ScrollRect scrollRect = null;

		[SerializeField]
		private DisneyStoreHomeSlider slider = null;

		private DisneyStoreController storeController;

		private void OnValidate()
		{
		}

		public void SetFranchises(List<DisneyStoreFranchiseDefinition> franchises, DisneyStoreController storeController, DisneyStoreDefinition storeDefinition)
		{
			this.storeController = storeController;
			slider.SetItems(storeDefinition.SliderPrefabs, storeController);
			loadFranchises(franchises);
		}

		private void loadFranchises(List<DisneyStoreFranchiseDefinition> franchises)
		{
			for (int i = 0; i < franchises.Count; i++)
			{
				if (franchises[i].HomePrefabKey != null && !string.IsNullOrEmpty(franchises[i].HomePrefabKey.Key))
				{
					CoroutineRunner.Start(loadFranchise(franchises[i]), this, "");
				}
			}
		}

		private IEnumerator loadFranchise(DisneyStoreFranchiseDefinition franchise)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(franchise.HomePrefabKey);
			yield return request;
			GameObject franchiseGo = Object.Instantiate(request.Asset, scrollRect.content, false);
			franchiseGo.GetComponent<DisneyStoreHomeFranchiseItem>().SetFranchise(franchise, storeController);
		}
	}
}

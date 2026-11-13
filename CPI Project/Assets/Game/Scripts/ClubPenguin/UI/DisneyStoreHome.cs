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
			preloadFranchiseContent(franchises);
			slider.SetItems(storeDefinition.SliderPrefabs, storeController);
			loadFranchises(franchises);
		}

		private void preloadFranchiseContent(List<DisneyStoreFranchiseDefinition> franchises)
		{
			Content.LoadAsync<GameObject>(DisneyStoreFranchise.DisneyStoreItemPrefabKey);
			Content.LoadAsync<GameObject>(DisneyStoreFranchise.PurchaseConfirmationPrefabKey);
			Content.LoadAsync<GameObject>(DisneyStoreFranchise.OwnedConfirmationPrefabKey);
			Content.LoadAsync<GameObject>(DisneyStoreFranchise.ConsumableConfirmationPrefabKey);
			Content.LoadAsync<GameObject>(DisneyStoreFranchise.IglooConfirmationPrefabKey);
			for (int i = 0; i < franchises.Count; i++)
			{
				DisneyStoreFranchiseDefinition franchise = franchises[i];
				if (franchise.HomePrefabKey != null && !string.IsNullOrEmpty(franchise.HomePrefabKey.Key))
				{
					Content.LoadAsync<GameObject>(franchise.HomePrefabKey);
				}
				preloadSprite(franchise.FranchiseIconPath);
				preloadSprite(franchise.FranchiseHeaderPath);
				if (franchise.Items == null)
				{
					continue;
				}
				for (int itemIndex = 0; itemIndex < franchise.Items.Count; itemIndex++)
				{
					preloadSprite(franchise.Items[itemIndex].Icon);
				}
			}
		}

		private void preloadSprite(TypedAssetContentKey<Sprite> contentKey)
		{
			if (contentKey != null && !string.IsNullOrEmpty(contentKey.Key))
			{
				Content.LoadAsync<Sprite>(contentKey);
			}
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

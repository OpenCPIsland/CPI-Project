using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Decoration : IRewardIconRenderer
	{
		private static PrefabContentKey decorationInstanceKey = new PrefabContentKey("Rewards/RewardPopup/RewardDecorationStructureInstancesItem");

		private static PrefabContentKey purchaseRightKey = new PrefabContentKey("Rewards/RewardPopup/RewardDecorationStructurePurchaseRightsItem");

		private DReward reward;

		private RewardIconRenderComplete callback;

		private GameObject itemPrefab;

		private DecorationDefinition decorationDefinition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.reward = reward;
			this.callback = callback;
			Dictionary<int, DecorationDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, DecorationDefinition>>();
			if (dictionary.TryGetValue((int)reward.UnlockID, out decorationDefinition))
			{
				CoroutineRunner.Start(renderDecorationInstance(), this, "renderDecorationInstance");
			}
		}

		private IEnumerator renderDecorationInstance()
		{
			AssetRequest<GameObject> prefabRequest = (reward.Category != RewardCategory.decorationPurchaseRights) ? Content.LoadAsync(decorationInstanceKey) : Content.LoadAsync(purchaseRightKey);
			yield return prefabRequest;
			GameObject prefab = prefabRequest.Asset;
			itemPrefab = Object.Instantiate(prefab);
			Content.LoadAsync(onDecorationInstanceLoaded, decorationDefinition.Icon);
		}

		private void onDecorationInstanceLoaded(string path, Texture2D texture)
		{
			Sprite icon = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
			int num = (reward.Category != RewardCategory.decorationPurchaseRights) ? ((int)reward.Data) : decorationDefinition.Cost;
			itemPrefab.GetComponent<RewardPopupLabelComponent>().Init(icon, num.ToString());
			callback(null, itemPrefab.GetComponent<RectTransform>(), decorationDefinition.Name);
		}
	}
}

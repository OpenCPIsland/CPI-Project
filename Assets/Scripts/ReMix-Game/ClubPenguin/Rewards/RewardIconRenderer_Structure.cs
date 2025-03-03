using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Structure : IRewardIconRenderer
	{
		private static PrefabContentKey structureInstanceKey = new PrefabContentKey("Rewards/RewardPopup/RewardDecorationStructureInstancesItem");

		private static PrefabContentKey purchaseRightKey = new PrefabContentKey("Rewards/RewardPopup/RewardDecorationStructurePurchaseRightsItem");

		private DReward reward;

		private RewardIconRenderComplete callback;

		private GameObject itemPrefab;

		private StructureDefinition structureDefinition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.reward = reward;
			this.callback = callback;
			Dictionary<int, StructureDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, StructureDefinition>>();
			if (dictionary.TryGetValue((int)reward.UnlockID, out structureDefinition))
			{
				CoroutineRunner.Start(renderStructureInstance(), this, "renderStructureInstance");
			}
		}

		private IEnumerator renderStructureInstance()
		{
			AssetRequest<GameObject> prefabRequest = (reward.Category != RewardCategory.structurePurchaseRights) ? Content.LoadAsync(structureInstanceKey) : Content.LoadAsync(purchaseRightKey);
			yield return prefabRequest;
			Texture2DContentKey iconContentKey = structureDefinition.Icon;
			AssetRequest<Texture2D> iconRequest = Content.LoadAsync(iconContentKey);
			yield return iconRequest;
			GameObject prefab = prefabRequest.Asset;
			Sprite sprite = Sprite.Create(iconRequest.Asset, new Rect(0f, 0f, iconRequest.Asset.width, iconRequest.Asset.height), Vector2.zero);
			int amount = (reward.Category != RewardCategory.structurePurchaseRights) ? ((int)reward.Data) : structureDefinition.Cost;
			itemPrefab = Object.Instantiate(prefab);
			itemPrefab.GetComponent<RewardPopupLabelComponent>().Init(sprite, amount.ToString());
			callback(null, itemPrefab.GetComponent<RectTransform>(), structureDefinition.Name);
		}
	}
}

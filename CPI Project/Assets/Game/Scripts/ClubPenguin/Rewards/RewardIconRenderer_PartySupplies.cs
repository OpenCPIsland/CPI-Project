using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_PartySupplies : IRewardIconRenderer
	{
		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardPartySuppliesItem");

		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			PropDefinition propByName = getPropByName((int)reward.UnlockID);
			if (propByName != null)
			{
				CoroutineRunner.Start(renderPartySupply(propByName), this, "LoadPropIcon");
			}
		}

		private IEnumerator renderPartySupply(PropDefinition propDefinition)
		{
			SpriteContentKey iconContentKey = propDefinition.IconContentKey;
			AssetRequest<Sprite> iconRequest = Content.LoadAsync(iconContentKey);
			yield return iconRequest;
			AssetRequest<GameObject> prefabRequest = Content.LoadAsync(prefabContentKey);
			yield return prefabRequest;
			GameObject prefab = prefabRequest.Asset;
			GameObject itemPrefab = Object.Instantiate(prefab);
			itemPrefab.GetComponent<RewardPopupLabelComponent>().Init(iconRequest.Asset, propDefinition.Cost.ToString());
			callback(null, itemPrefab.GetComponent<RectTransform>(), propDefinition.Name);
		}

		private PropDefinition getPropByName(int propID)
		{
			Dictionary<int, PropDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			PropDefinition value;
			if (!dictionary.TryGetValue(propID, out value))
			{
				return null;
			}
			return value;
		}
	}
}

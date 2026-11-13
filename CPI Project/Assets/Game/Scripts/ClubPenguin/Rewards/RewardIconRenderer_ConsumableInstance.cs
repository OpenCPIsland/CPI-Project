using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_ConsumableInstance : IRewardIconRenderer
	{
		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardConsumablesItem");

		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			PropDefinition propDefinition = Service.Get<PropService>().GetPropDefinition(reward.UnlockID.ToString());
			if (propDefinition != null)
			{
				CoroutineRunner.Start(renderPropIcon(propDefinition, (int)reward.Data), this, "LoadPropIcon");
			}
		}

		private IEnumerator renderPropIcon(PropDefinition propDefinition, int count)
		{
			SpriteContentKey iconContentKey = propDefinition.IconContentKey;
			AssetRequest<Sprite> iconRequest = Content.LoadAsync(iconContentKey);
			yield return iconRequest;
			AssetRequest<GameObject> prefabRequest = Content.LoadAsync(prefabContentKey);
			yield return prefabRequest;
			GameObject prefab = prefabRequest.Asset;
			GameObject itemPrefab = Object.Instantiate(prefab);
			itemPrefab.GetComponent<RewardPopupLabelComponent>().Init(iconRequest.Asset, count.ToString());
			callback(null, itemPrefab.GetComponent<RectTransform>(), propDefinition.Name);
		}
	}
}

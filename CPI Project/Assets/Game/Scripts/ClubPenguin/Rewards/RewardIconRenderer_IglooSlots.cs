using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_IglooSlots : IRewardIconRenderer
	{
		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardIglooSlotItem");

		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderIglooSlot(), this, "renderIglooSlot");
		}

		private IEnumerator renderIglooSlot()
		{
			AssetRequest<GameObject> prefabRequest = Content.LoadAsync(prefabContentKey);
			yield return prefabRequest;
			GameObject prefab = prefabRequest.Asset;
			GameObject itemPrefab = Object.Instantiate(prefab);
			callback(null, itemPrefab.GetComponent<RectTransform>());
		}
	}
}

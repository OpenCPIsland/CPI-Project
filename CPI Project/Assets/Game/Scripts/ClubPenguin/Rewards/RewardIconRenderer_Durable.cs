using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Durable : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			int unlockID = (!(reward.UnlockID is string)) ? ((int)reward.UnlockID) : int.Parse((string)reward.UnlockID);
			this.callback = callback;
			CoroutineRunner.Start(renderDurable(unlockID), this, "RewardIconRenderer_Durable.renderDurable");
		}

		private IEnumerator renderDurable(int unlockID)
		{
			Dictionary<int, PropDefinition> decals = Service.Get<GameData>().Get<Dictionary<int, PropDefinition>>();
			SpriteContentKey iconContentKey = new SpriteContentKey(RewardPopupConstants.DefaultIconContentKey.Key);
			PropDefinition definition;
			if (decals.TryGetValue(unlockID, out definition))
			{
				iconContentKey = definition.GetIconContentKey();
			}
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(assetRequest.Asset, null);
		}
	}
}

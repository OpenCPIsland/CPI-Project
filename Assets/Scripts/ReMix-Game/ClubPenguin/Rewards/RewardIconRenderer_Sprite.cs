using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Sprite : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderSprite((SpriteContentKey)reward.UnlockID), this, "RewardIconRenderer_Sprite.renderSprite");
		}

		private IEnumerator renderSprite(SpriteContentKey unlockID)
		{
			AssetRequest<Sprite> assetRequest = Content.LoadAsync(unlockID);
			yield return assetRequest;
			callback(assetRequest.Asset, null);
		}
	}
}

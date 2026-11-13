using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_ColourPack : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderColourPack((string)reward.UnlockID), this, "RewardIconRenderer_ColourPack.renderColourPack");
		}

		private IEnumerator renderColourPack(string unlockID)
		{
			Texture2DContentKey iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

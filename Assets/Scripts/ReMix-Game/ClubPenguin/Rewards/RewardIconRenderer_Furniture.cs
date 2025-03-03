using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Furniture : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderFurnitureTemplate((string)reward.UnlockID), this, "RewardIconRenderer_Furniture.renderFurnitureTemplate");
		}

		private IEnumerator renderFurnitureTemplate(string unlockID)
		{
			Texture2DContentKey iconContentKey = EquipmentPathUtil.GetFurnitureIconPath(unlockID);
			if (!Content.ContainsKey(iconContentKey.Key))
			{
				iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			}
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

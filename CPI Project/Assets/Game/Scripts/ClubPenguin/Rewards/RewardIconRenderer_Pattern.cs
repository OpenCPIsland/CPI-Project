using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Pattern : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderPattern((int)reward.UnlockID), this, "RewardIconRenderer_Decal.renderDecal");
		}

		private IEnumerator renderPattern(int unlockID)
		{
			Dictionary<int, FabricDefinition> decals = Service.Get<GameData>().Get<Dictionary<int, FabricDefinition>>();
			Texture2DContentKey iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			FabricDefinition definition;
			if (decals.TryGetValue(unlockID, out definition))
			{
				iconContentKey = EquipmentPathUtil.GetFabricPath(definition.AssetName);
			}
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

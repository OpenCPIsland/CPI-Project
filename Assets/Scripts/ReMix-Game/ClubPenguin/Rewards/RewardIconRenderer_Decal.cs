using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Decal : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderDecal((int)reward.UnlockID), this, "RewardIconRenderer_Decal.renderDecal");
		}

		private IEnumerator renderDecal(int decalID)
		{
			Dictionary<int, DecalDefinition> decals = Service.Get<GameData>().Get<Dictionary<int, DecalDefinition>>();
			Texture2DContentKey iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			DecalDefinition decalDefinition;
			if (decals.TryGetValue(decalID, out decalDefinition))
			{
				iconContentKey = EquipmentPathUtil.GetDecalPath(decalDefinition.AssetName);
			}
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

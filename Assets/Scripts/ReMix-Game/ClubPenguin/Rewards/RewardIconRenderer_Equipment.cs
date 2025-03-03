using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Equipment : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			CoroutineRunner.Start(renderClothingTemplate((int)reward.UnlockID), this, "RewardIconRenderer_Equipment.renderClothingTemplate");
		}

		private IEnumerator renderClothingTemplate(int unlockID)
		{
			Dictionary<int, TemplateDefinition> templates = Service.Get<GameData>().Get<Dictionary<int, TemplateDefinition>>();
			Texture2DContentKey iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			TemplateDefinition definition;
			if (templates.TryGetValue(unlockID, out definition))
			{
				iconContentKey = EquipmentPathUtil.GetEquipmentIconPath(definition.AssetName);
			}
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null, definition.Name);
		}
	}
}

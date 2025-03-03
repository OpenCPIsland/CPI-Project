using ClubPenguin.DecorationInventory;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Lighting : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		private LightingDefinition lightingDefinition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			Dictionary<int, LightingDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, LightingDefinition>>();
			if (dictionary.TryGetValue((int)reward.UnlockID, out lightingDefinition))
			{
				CoroutineRunner.Start(renderLighting(), this, "renderLighting");
			}
		}

		private IEnumerator renderLighting()
		{
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(lightingDefinition.Icon);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

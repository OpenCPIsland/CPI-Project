using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Lot : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		private LotDefinition lotDefinition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			Dictionary<string, LotDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, LotDefinition>>();
			if (dictionary.TryGetValue((string)reward.UnlockID, out lotDefinition))
			{
				CoroutineRunner.Start(renderLot(), this, "renderLot");
			}
		}

		private IEnumerator renderLot()
		{
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(lotDefinition.PreviewImageLarge);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

using ClubPenguin.Chat;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_Emote : IRewardIconRenderer
	{
		private RewardIconRenderComplete callback;

		private SpriteContentKey emoteContentKey = new SpriteContentKey("Rewards/Sprites/*");

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			EmoteDefinition emoteByName = getEmoteByName((string)reward.UnlockID);
			if (emoteByName != null)
			{
				CoroutineRunner.Start(renderEmote(emoteByName), this, "");
				return;
			}
			Log.LogError(this, "Unable to find emote definition of name: " + reward.UnlockID);
			CoroutineRunner.Start(loadDefaultIcon(), this, "");
		}

		private IEnumerator renderEmote(EmoteDefinition emoteDefinition)
		{
			AssetRequest<Sprite> request = Content.LoadAsync(emoteContentKey, emoteDefinition.Id);
			yield return request;
			callback(request.Asset, null);
		}

		private EmoteDefinition getEmoteByName(string emoteName)
		{
			Dictionary<string, EmoteDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, EmoteDefinition>>();
			EmoteDefinition value;
			dictionary.TryGetValue(emoteName, out value);
			return value;
		}

		private IEnumerator loadDefaultIcon()
		{
			Texture2DContentKey iconContentKey = RewardPopupConstants.DefaultIconContentKey;
			AssetRequest<Texture2D> assetRequest = Content.LoadAsync(iconContentKey);
			yield return assetRequest;
			callback(Sprite.Create(assetRequest.Asset, new Rect(0f, 0f, assetRequest.Asset.width, assetRequest.Asset.height), Vector2.zero), null);
		}
	}
}

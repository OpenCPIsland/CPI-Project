using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_SizzleClips : IRewardIconRenderer
	{
		public static readonly SpriteContentKey SizzleClipIconContentKey = new SpriteContentKey("SizzleClipIcons/*");

		private RewardIconRenderComplete callback;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			SizzleClipDefinition sizzleClipByName = getSizzleClipByName((int)reward.UnlockID);
			if (sizzleClipByName != null)
			{
				Content.LoadAsync(onLoadComplete, SizzleClipIconContentKey, sizzleClipByName.name);
			}
			else
			{
				Content.LoadAsync(onDefaultLoadComplete, RewardPopupConstants.DefaultIconContentKey);
			}
		}

		private void onLoadComplete(string path, Sprite sprite)
		{
			callback(sprite, null);
		}

		private void onDefaultLoadComplete(string path, Texture2D texture)
		{
			callback(Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero), null);
		}

		private SizzleClipDefinition getSizzleClipByName(int sizzleID)
		{
			Dictionary<int, SizzleClipDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, SizzleClipDefinition>>();
			SizzleClipDefinition value;
			dictionary.TryGetValue(sizzleID, out value);
			return value;
		}
	}
}

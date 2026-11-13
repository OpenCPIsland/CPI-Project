using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardIconRenderer_MusicTracks : IRewardIconRenderer
	{
		private static PrefabContentKey prefabContentKey = new PrefabContentKey("Rewards/RewardPopup/RewardMusicTracksItem");

		private RewardIconRenderComplete callback;

		private MusicTrackDefinition musicTrackDefinition;

		public void RenderReward(DReward reward, RewardIconRenderComplete callback)
		{
			this.callback = callback;
			Dictionary<int, MusicTrackDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<int, MusicTrackDefinition>>();
			if (dictionary.TryGetValue((int)reward.UnlockID, out musicTrackDefinition))
			{
				CoroutineRunner.Start(renderMusicTracks(), this, "renderMusicTracks");
			}
		}

		private IEnumerator renderMusicTracks()
		{
			AssetRequest<GameObject> prefabRequest = Content.LoadAsync(prefabContentKey);
			yield return prefabRequest;
			AssetRequest<Texture2D> iconRequest = null;
			if (musicTrackDefinition.Icon != null && !string.IsNullOrEmpty(musicTrackDefinition.Icon.Key))
			{
				iconRequest = Content.LoadAsync(musicTrackDefinition.Icon);
				yield return iconRequest;
			}
			GameObject prefab = prefabRequest.Asset;
			GameObject itemPrefab = Object.Instantiate(prefab);
			Sprite sprite = null;
			if (iconRequest != null && iconRequest.Asset != null)
			{
				sprite = Sprite.Create(iconRequest.Asset, new Rect(0f, 0f, iconRequest.Asset.width, iconRequest.Asset.height), Vector2.zero);
			}
			itemPrefab.GetComponent<RewardPopupLabelComponent>().Init(sprite, Service.Get<Localizer>().GetTokenTranslation(musicTrackDefinition.Name));
			callback(null, itemPrefab.GetComponent<RectTransform>());
		}
	}
}

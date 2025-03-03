using ClubPenguin.Net.Domain;
using ClubPenguin.Rewards;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowRewardPopup
{
	public class Builder
	{
		public DRewardPopup.RewardPopupType Type
		{
			get;
			private set;
		}

		public Reward RewardData
		{
			get;
			private set;
		}

		public string MascotName
		{
			get;
			private set;
		}

		public string HeaderText
		{
			get;
			private set;
		}

		public string SourceID
		{
			get;
			private set;
		}

		public bool ShowXpAndCoinsUI
		{
			get;
			private set;
		}

		public string RewardPopupPrefabOverride
		{
			get;
			private set;
		}

		public List<PrefabContentKey> CustomScreenKeys
		{
			get;
			private set;
		}

		public Builder(DRewardPopup.RewardPopupType type, Reward rewardData)
		{
			Type = type;
			RewardData = rewardData;
			ShowXpAndCoinsUI = true;
		}

		public Builder setMascotName(string mascotName)
		{
			MascotName = mascotName;
			return this;
		}

		public Builder setHeaderText(string headerText)
		{
			HeaderText = headerText;
			return this;
		}

		public Builder setShowXpAndCoinsUI(bool show)
		{
			ShowXpAndCoinsUI = show;
			return this;
		}

		public Builder setRewardSource(string sourceID)
		{
			SourceID = sourceID;
			return this;
		}

		public Builder setRewardPopupPrefabOverride(string prefabPath)
		{
			RewardPopupPrefabOverride = prefabPath;
			return this;
		}

		public Builder setCustomScreenKey(PrefabContentKey customScreenKey)
		{
			CustomScreenKeys = new List<PrefabContentKey>();
			CustomScreenKeys.Add(customScreenKey);
			return this;
		}

		public Builder setCustomScreenKeys(List<PrefabContentKey> customScreenKeys)
		{
			CustomScreenKeys = customScreenKeys;
			return this;
		}

		public ShowRewardPopup Build()
		{
			return new ShowRewardPopup(this);
		}
	}

	private DRewardPopup popupData;

	private ShowRewardPopup(Builder builder)
	{
		popupData = new DRewardPopup();
		popupData.PopupType = builder.Type;
		popupData.RewardData = builder.RewardData;
		popupData.SplashTitleToken = builder.HeaderText;
		popupData.MascotName = builder.MascotName;
		popupData.ShowXpAndCoinsUI = builder.ShowXpAndCoinsUI;
		popupData.SourceID = builder.SourceID;
		popupData.RewardPopupPrefabOverride = builder.RewardPopupPrefabOverride;
		popupData.CustomScreenKeys = builder.CustomScreenKeys;
	}

	public void Execute()
	{
		CoroutineRunner.Start(loadRewardPopup(), this, "loadRewardPopup");
	}

	private IEnumerator loadRewardPopup()
	{
		PrefabContentKey contentKey = RewardPopupConstants.RewardPopupContentKey;
		if (!string.IsNullOrEmpty(popupData.RewardPopupPrefabOverride))
		{
			contentKey = new PrefabContentKey(popupData.RewardPopupPrefabOverride);
		}
		AssetRequest<GameObject> assetRequest = Content.LoadAsync(contentKey);
		yield return assetRequest;
		GameObject popup = Object.Instantiate(assetRequest.Asset);
		popup.GetComponent<RewardPopupController>().Init(popupData);
		Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(popup, false, true, "Accessibility.Popup.Title.RewardPopup"));
	}
}

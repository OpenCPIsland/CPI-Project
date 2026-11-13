using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.PartyGames;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameEndGamePopup : BasicPopup
	{
		protected enum PlayerPlacementType
		{
			WIN,
			LOSS,
			TIE
		}

		private const string CLAIM_TOKEN = "Rewards.RewardPopupScreen.ClaimButton";

		private const string OK_TOKEN = "GlobalUI.Buttons.OKText";

		[Space(10f)]
		public Text TitleText;

		public GameObject ResultsPanel;

		public GameObject RewardsPanel;

		public Text CoinsText;

		public Text XpText;

		public GameObject CoinsPanel;

		public GameObject XpPanel;

		public GameObject Seperator;

		public Text ClaimButtonText;

		[LocalizationToken]
		public string WinTitleToken;

		[LocalizationToken]
		public string LoseTitleToken;

		[LocalizationToken]
		public string TieTitleToken;

		public PrefabContentKey PlayerItemPrefab;

		[Space(10f)]
		public string WinSFXTrigger;

		public string LossSFXTrigger;

		public string TieSFXTrigger;

		private Localizer localizer;

		private PartyGameEndGamePlayerData localPlayerData;

		private long gameSessionId;

		protected override void awake()
		{
			localizer = Service.Get<Localizer>();
		}

		protected override void start()
		{
		}

		public void SetPlayerResults(PartyGameEndGamePlayerData[] orderedPlayerData, PartyGameDefinition definition, long gameSessionId)
		{
			localPlayerData = default(PartyGameEndGamePlayerData);
			this.gameSessionId = gameSessionId;
			for (int i = 0; i < orderedPlayerData.Length; i++)
			{
				if (orderedPlayerData[i].IsLocalPlayer)
				{
					localPlayerData = orderedPlayerData[i];
					break;
				}
			}
			string token = WinTitleToken;
			switch ((localPlayerData.Placement == -1) ? PlayerPlacementType.LOSS : getLocalPlayerPlacementType(orderedPlayerData))
			{
			case PlayerPlacementType.WIN:
				if (!string.IsNullOrEmpty(WinSFXTrigger))
				{
					EventManager.Instance.PostEvent(WinSFXTrigger, EventAction.PlaySound);
				}
				break;
			case PlayerPlacementType.LOSS:
				if (!string.IsNullOrEmpty(LossSFXTrigger))
				{
					EventManager.Instance.PostEvent(LossSFXTrigger, EventAction.PlaySound);
				}
				token = LoseTitleToken;
				break;
			case PlayerPlacementType.TIE:
				if (!string.IsNullOrEmpty(TieSFXTrigger))
				{
					EventManager.Instance.PostEvent(TieSFXTrigger, EventAction.PlaySound);
				}
				token = TieTitleToken;
				break;
			}
			TitleText.text = localizer.GetTokenTranslation(token);
			Reward reward = null;
			for (int i = 0; i < definition.Rewards.Count; i++)
			{
				if (definition.Rewards[i].Placement == (PartyGameEndPlacement)localPlayerData.Placement)
				{
					reward = definition.Rewards[i].Reward.ToReward();
				}
			}
			if (reward != null)
			{
				RewardsPanel.SetActive(true);
				CoinReward rewardable;
				if (reward.TryGetValue(out rewardable) && !rewardable.IsEmpty())
				{
					CoinsText.text = rewardable.Coins.ToString();
				}
				else
				{
					CoinsPanel.SetActive(false);
					Seperator.SetActive(false);
				}
				MascotXPReward rewardable2;
				if (reward.TryGetValue(out rewardable2))
				{
					using (Dictionary<string, int>.ValueCollection.Enumerator enumerator = rewardable2.XP.Values.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							int current = enumerator.Current;
							XpText.text = current.ToString();
						}
					}
				}
				else
				{
					XpPanel.SetActive(false);
					Seperator.SetActive(false);
				}
				ClaimButtonText.text = localizer.GetTokenTranslation("Rewards.RewardPopupScreen.ClaimButton");
			}
			else
			{
				RewardsPanel.SetActive(false);
				ClaimButtonText.text = localizer.GetTokenTranslation("GlobalUI.Buttons.OKText");
			}
			CoroutineRunner.Start(loadResultsItems(orderedPlayerData), this, "LoadEndGameResultItem");
		}

		private IEnumerator loadResultsItems(PartyGameEndGamePlayerData[] orderedPlayerData)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(PlayerItemPrefab);
			yield return assetRequest;
			for (int i = 0; i < orderedPlayerData.Length; i++)
			{
				PartyGameEndGamePlayerItem component = Object.Instantiate(assetRequest.Asset, ResultsPanel.transform, false).GetComponent<PartyGameEndGamePlayerItem>();
				component.SetPlayerData(orderedPlayerData[i]);
			}
		}

		private void OnDestroy()
		{
			if (localPlayerData.Placement == 0)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyB));
			}
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDelayedReward(RewardSource.MINI_GAME, gameSessionId.ToString()));
		}

		protected PlayerPlacementType getLocalPlayerPlacementType(PartyGameEndGamePlayerData[] orderedPlayerData)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			int num3 = 0;
			for (int i = 0; i < orderedPlayerData.Length; i++)
			{
				PartyGameEndGamePlayerData partyGameEndGamePlayerData = orderedPlayerData[i];
				if (partyGameEndGamePlayerData.IsLocalPlayer)
				{
					flag = true;
					num3 = partyGameEndGamePlayerData.Placement;
				}
				if (num == 0)
				{
					num2 = partyGameEndGamePlayerData.Placement;
					num++;
					continue;
				}
				if (partyGameEndGamePlayerData.Placement == num2)
				{
					num++;
					continue;
				}
				if (flag)
				{
					break;
				}
				num = 1;
				num2 = partyGameEndGamePlayerData.Placement;
			}
			if (num > 1)
			{
				if (orderedPlayerData.Length > 2)
				{
					if (num2 == 0)
					{
						return PlayerPlacementType.WIN;
					}
					return PlayerPlacementType.LOSS;
				}
				return PlayerPlacementType.TIE;
			}
			if (num3 == 0)
			{
				return PlayerPlacementType.WIN;
			}
			return PlayerPlacementType.LOSS;
		}
	}
}

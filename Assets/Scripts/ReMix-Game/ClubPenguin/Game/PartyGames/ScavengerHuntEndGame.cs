using ClubPenguin.Adventure;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.NPC;
using ClubPenguin.Progression;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class ScavengerHuntEndGame : MonoBehaviour
	{
		private const string WINNER_TOKEN = "PartyGames.ScavengerHunt.Winner";

		private const string TRY_AGAIN_TOKEN = "PartyGames.ScavengerHunt.TryAgain";

		private const string YOUR_REWARDS_TOKEN = "PartyGames.ScavengerHunt.YourRewards";

		private const string DIDNT_HIDE_TOKEN = "PartyGames.ScavengerHunt.DidntHideMessage";

		private const string YOU_FOUND_TOKEN = "PartyGames.ScavengerHunt.YouFound";

		private const string PLAYER_FOUND_TOKEN = "PartyGames.ScavengerHunt.PlayerFound";

		private const string NONE_FOUND_TOKEN = "Activity.ScavengerHunt.NoneFound";

		private const string SEEKER_FAILED_TOKEN = "Activity.ScavengerHunt.SeekerFailed";

		private const int TORSO_LAYER = 1;

		private readonly PrefabContentKey PLAYER_RESULTS_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntPlayerResults");

		public Text HeaderTitle;

		public Transform PlayerResultsContent;

		public Text SingleMessageText;

		public Text YourRewardsText;

		public Text CoinText;

		public GameObject WinnerFX;

		public Text XPText;

		public GameObject XPPanel;

		public GameObject RewardsPanel;

		public GameObject MaxLevelText;

		private ScavengerHuntData scavengerHuntData;

		private int totalItemsHidden;

		private int totalItemsFound;

		private bool isWinner;

		public void OnClaimButton()
		{
			try
			{
				Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDelayedReward(RewardSource.MINI_GAME, scavengerHuntData.GameSessionId.ToString()));
				playEndingPenguinAnimations();
				if (isWinner)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyB));
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void Init(ScavengerHuntData scavengerHuntData, int totalItemsHidden, int totalItemsFound, bool isWinner)
		{
			this.scavengerHuntData = scavengerHuntData;
			this.totalItemsHidden = totalItemsHidden;
			this.totalItemsFound = totalItemsFound;
			this.isWinner = isWinner;
			if (isWinner)
			{
				HeaderTitle.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.Winner");
				if (WinnerFX != null)
				{
					WinnerFX.SetActive(true);
				}
			}
			else
			{
				HeaderTitle.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.TryAgain");
			}
			setRewards();
			if (totalItemsHidden == 0)
			{
				SingleMessageText.gameObject.SetActive(true);
				if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Finder)
				{
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.DidntHideMessage");
					SingleMessageText.text = string.Format(tokenTranslation, scavengerHuntData.OtherPlayerName);
				}
				else
				{
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.DidntHideMessage");
					SingleMessageText.text = string.Format(tokenTranslation, scavengerHuntData.LocalPlayerName);
				}
			}
			else if (totalItemsFound == 0)
			{
				SingleMessageText.gameObject.SetActive(true);
				if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Finder)
				{
					SingleMessageText.text = Service.Get<Localizer>().GetTokenTranslation("Activity.ScavengerHunt.NoneFound");
					return;
				}
				string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("Activity.ScavengerHunt.SeekerFailed");
				SingleMessageText.text = string.Format(tokenTranslation2, scavengerHuntData.OtherPlayerName);
			}
			else
			{
				SingleMessageText.gameObject.SetActive(false);
				Content.LoadAsync(onPlayerResultsLoaded, PLAYER_RESULTS_PREFAB_KEY);
			}
		}

		private void playEndingPenguinAnimations()
		{
			if (isWinner)
			{
				scavengerHuntData.LocalPlayerAnimator.Play("Celebration", 1);
				if (scavengerHuntData.OtherPlayerAnimator != null)
				{
					scavengerHuntData.OtherPlayerAnimator.Play("Lose", 1);
				}
			}
			else
			{
				scavengerHuntData.LocalPlayerAnimator.Play("Lose", 1);
				if (scavengerHuntData.OtherPlayerAnimator != null)
				{
					scavengerHuntData.OtherPlayerAnimator.Play("Celebration", 1);
				}
			}
		}

		private void onPlayerResultsLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, PlayerResultsContent, false);
			ScavengerHuntPlayerResults component = gameObject.GetComponent<ScavengerHuntPlayerResults>();
			string text = "";
			if (scavengerHuntData.LocalPlayerRole == ScavengerHunt.ScavengerHuntRoles.Finder)
			{
				text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.YouFound");
			}
			else
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.PlayerFound");
				text = string.Format(tokenTranslation, scavengerHuntData.OtherPlayerName);
			}
			int num = totalItemsFound + (scavengerHuntData.TotalMarbleCount - totalItemsHidden);
			component.Init(scavengerHuntData.TotalMarbleCount, num, text);
		}

		private void setRewards()
		{
			ProgressionService progressionService = Service.Get<ProgressionService>();
			YourRewardsText.text = Service.Get<Localizer>().GetTokenTranslation("PartyGames.ScavengerHunt.YourRewards");
			CPRewardDefinition cPRewardDefinition = isWinner ? ((CPRewardDefinition)scavengerHuntData.WinRewardDefinition) : ((CPRewardDefinition)scavengerHuntData.LoseRewardDefinition);
			CoinText.text = cPRewardDefinition.Coins.Amount.ToString();
			bool flag = false;
			int num = 0;
			MascotXPRewardDefinition[] mascotXP = cPRewardDefinition.MascotXP;
			for (int i = 0; i < mascotXP.Length; i++)
			{
				num += mascotXP[i].XP;
				MascotDefinition mascot = mascotXP[i].Mascot;
				flag = progressionService.IsMascotMaxLevel(mascot.name);
			}
			if (num > 0)
			{
				if (flag)
				{
					XPText.gameObject.SetActive(false);
					MaxLevelText.SetActive(true);
				}
				else
				{
					XPText.text = num.ToString();
				}
			}
			else
			{
				XPPanel.SetActive(false);
			}
		}
	}
}

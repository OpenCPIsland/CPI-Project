using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameTeamEndGamePopup : MonoBehaviour
	{
		private const string WINNER_TOKEN = "PartyGames.ScavengerHunt.Winner";

		private const string LOSER_TOKEN = "Activity.DanceBattle.Lose";

		private const string TIE_TOKEN = "PartyGames.FindFour.Draw";

		public Transform TeamItemParent;

		public PartyGameTeamEndGamePopupReward RewardDisplay;

		public PrefabContentKey TeamItemContentKey;

		public TintSelector HeaderTintSelector;

		public SpriteSelector BackgroundSpriteSelector;

		public GameObjectSelector ParticleSelector;

		public Text HeaderText;

		private PartyGameTeamEndGamePopupData data;

		private Reward localPlayerReward;

		private List<PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData> teamData;

		private PartyGameTeamEndGamePopupData.LocalPlayerResult result;

		public void SetData(PartyGameTeamEndGamePopupData data)
		{
			localPlayerReward = data.LocalPlayerReward;
			teamData = data.TeamData;
			this.data = data;
			result = data.getLocalPlayerResult();
			loadTeamItemPrefab();
			RewardDisplay.SetReward(localPlayerReward);
			setTheme(data.ThemeId);
			setHeaderText(result);
		}

		private void OnDestroy()
		{
			if (result == PartyGameTeamEndGamePopupData.LocalPlayerResult.Win)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new HeadStatusEvents.ShowHeadStatus(TemporaryHeadStatusType.TrophyB));
			}
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDelayedReward(RewardSource.MINI_GAME, data.GameSessionId.ToString()));
		}

		private void setTheme(int themeId)
		{
			HeaderTintSelector.SelectColor(themeId);
			BackgroundSpriteSelector.SelectSprite(themeId);
			ParticleSelector.SelectGameObject(themeId);
		}

		private void setHeaderText(PartyGameTeamEndGamePopupData.LocalPlayerResult result)
		{
			string token;
			switch (result)
			{
			case PartyGameTeamEndGamePopupData.LocalPlayerResult.Lose:
				token = "Activity.DanceBattle.Lose";
				break;
			case PartyGameTeamEndGamePopupData.LocalPlayerResult.Win:
				token = "PartyGames.ScavengerHunt.Winner";
				break;
			case PartyGameTeamEndGamePopupData.LocalPlayerResult.Tie:
				token = "PartyGames.FindFour.Draw";
				break;
			default:
				token = "PartyGames.FindFour.Draw";
				break;
			}
			HeaderText.text = Service.Get<Localizer>().GetTokenTranslation(token);
		}

		private void loadTeamItemPrefab()
		{
			Content.LoadAsync(onTeamItemLoadComplete, TeamItemContentKey);
		}

		private void onTeamItemLoadComplete(string path, GameObject teamItemPrefab)
		{
			loadTeamItems(teamData, teamItemPrefab);
		}

		private void loadTeamItems(List<PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData> teamData, GameObject teamItemPrefab)
		{
			for (int i = 0; i < teamData.Count; i++)
			{
				loadTeamItem(teamData[i], teamItemPrefab);
			}
		}

		private void loadTeamItem(PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData teamData, GameObject teamItemPrefab)
		{
			GameObject gameObject = Object.Instantiate(teamItemPrefab, TeamItemParent);
			gameObject.GetComponent<PartyGameTeamEndGamePopupTeam>().SetTeamData(teamData);
		}
	}
}

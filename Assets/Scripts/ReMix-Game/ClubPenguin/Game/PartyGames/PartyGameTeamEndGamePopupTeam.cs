using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameTeamEndGamePopupTeam : MonoBehaviour
	{
		public Text TeamNameText;

		public Text TeamScoreText;

		public GameObject TeamScorePanel;

		public GameObject WinPanel;

		public TintSelector BackgroundTintSelector;

		public SpriteSelector TeamIconSpriteSelector;

		public void SetTeamData(PartyGameTeamEndGamePopupData.PartyGameTeamEndGamePopupTeamData teamData)
		{
			TeamNameText.text = Service.Get<Localizer>().GetTokenTranslation(teamData.TeamNameToken);
			if (teamData.IsShowingScore)
			{
				TeamScorePanel.SetActive(true);
				TeamScoreText.text = teamData.Score.ToString();
			}
			else if (TeamScorePanel != null)
			{
				TeamScorePanel.SetActive(false);
			}
			WinPanel.SetActive(teamData.IsWinningTeam);
			if (teamData.IsLocalPlayersTeam)
			{
				BackgroundTintSelector.SelectColor(teamData.TeamThemeId);
			}
			else
			{
				BackgroundTintSelector.GetComponent<Image>().enabled = false;
			}
			TeamIconSpriteSelector.SelectSprite(teamData.TeamThemeId);
		}
	}
}

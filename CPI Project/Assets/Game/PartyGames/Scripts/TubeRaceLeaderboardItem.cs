using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceLeaderboardItem : MonoBehaviour
	{
		private const string DEFAULT_PLAYER_NAME = "";

		public Text PlayerNameText;

		public Text ScoreText;

		private CPDataEntityCollection dataEntityCollection;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public void SetData(long sessionId, int score)
		{
			string playerName = getPlayerName(sessionId);
			if (playerName != "")
			{
				PlayerNameText.text = playerName;
				ScoreText.text = score.ToString();
				base.gameObject.SetActive(true);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		private string getPlayerName(long sessionId)
		{
			string result = "";
			DataEntityHandle dataEntityHandle;
			DisplayNameData component;
			if (dataEntityCollection.TryFindEntity<SessionIdData, long>(sessionId, out dataEntityHandle) && dataEntityCollection.TryGetComponent(dataEntityHandle, out component))
			{
				result = component.DisplayName;
			}
			return result;
		}
	}
}

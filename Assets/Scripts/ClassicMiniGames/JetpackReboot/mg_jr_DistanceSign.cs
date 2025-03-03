using DevonLocalization.Core;
using Disney.MobileNetwork;
using MinigameFramework;
using UnityEngine;
using UnityEngine.UI;

namespace JetpackReboot
{
	public class mg_jr_DistanceSign : MonoBehaviour
	{
		private const float m_visualCenterOffSet = 0.7f;

		private Text m_distanceLabel;

		private Text m_signLabel;

		private string m_distanceUnit = "";

		private mg_JetpackReboot m_miniGame;

		private mg_jr_Odometer m_odometer;

		private void Start()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			m_odometer = m_miniGame.GameLogic.Odometer;
			Localizer localizer = null;
			if (Service.IsSet<Localizer>())
			{
				localizer = Service.Get<Localizer>();
			}
			m_distanceUnit = ((localizer == null) ? "m" : localizer.GetTokenTranslation("Activity.MiniGames.MetersDistance"));
			if (m_miniGame.PlayerStats.BestDistance == 0)
			{
				base.gameObject.SetActive(false);
				return;
			}
			Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
			Text[] array = componentsInChildren;
			foreach (Text text in array)
			{
				if (text.gameObject.name == "mg_jr_DistanceLabel")
				{
					m_distanceLabel = text;
					m_distanceLabel.text = m_miniGame.PlayerStats.BestDistance + m_distanceUnit;
				}
				else if (text.gameObject.name == "mg_jr_SignTitle")
				{
					m_signLabel = text;
					m_signLabel.text = ((localizer == null) ? "Longest Flight" : localizer.GetTokenTranslation("Activity.MiniGames.Flight"));
				}
			}
		}

		private void Update()
		{
			if (!m_miniGame.IsPaused && m_miniGame.GameLogic.IsGameInProgress)
			{
				float num = m_miniGame.GameLogic.Player.transform.position.x + 0.7f;
				mg_jr_GameData gameBalance = m_miniGame.GameLogic.GameBalance;
				float num2 = gameBalance.MetersToUnits(m_miniGame.PlayerStats.BestDistance);
				float num3 = gameBalance.MetersToUnits(m_odometer.DistanceTravelledThisRun);
				float x = num + (num2 - num3);
				Vector3 vector = m_miniGame.MainCamera.WorldToScreenPoint(new Vector3(x, 0f, 0f));
				Vector3 position = new Vector3(vector.x, base.transform.position.y, 0f);
				base.transform.position = position;
			}
		}
	}
}

using ClubPenguin.Classic;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_ResultScreen : MinigameScreen
{
	private Text m_coinsEarnedLabel;

	private Text m_currentCoinsLabel;

	private Text m_gameNameLabel;

	private Button m_ok;

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "btn_exit")
			{
				m_ok = button;
			}
		}
		Text[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Text>();
		Text[] array2 = componentsInChildren2;
		foreach (Text text in array2)
		{
			if (text.gameObject.name == "txt_coins")
			{
				m_coinsEarnedLabel = text;
			}
			else if (text.gameObject.name == "txt_balance")
			{
				m_currentCoinsLabel = text;
			}
			else if (text.gameObject.name == "txt_name")
			{
				m_gameNameLabel = text;
			}
		}
		m_ok.onClick.AddListener(OnOKClicked);
		ClassicMiniGames.PushBackButtonHandler(OnOKClicked);
		UpdateLabels();
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_ok.onClick.RemoveListener(OnOKClicked);
		m_ok = null;
		ClassicMiniGames.RemoveBackButtonHandler(OnOKClicked);
	}

	private void UpdateLabels()
	{
		Minigame active = MinigameManager.GetActive();
		int coinsEarned = active.CoinsEarned;
		int playerCoins = MinigameManager.Instance.PlayerCoins;
		if (Service.IsSet<Localizer>())
		{
			m_coinsEarnedLabel.text = string.Format(Service.Get<Localizer>().GetTokenTranslation("Activity.MiniGames.Coins"), coinsEarned);
			m_currentCoinsLabel.text = Service.Get<Localizer>().GetTokenTranslation("Activity.MiniGames.TotalCoins") + playerCoins;
		}
		else
		{
			m_coinsEarnedLabel.text = coinsEarned + " coins!";
			m_currentCoinsLabel.text = "Your total coins: " + playerCoins;
		}
		m_gameNameLabel.text = active.GetMinigameName();
	}

	private void OnOKClicked()
	{
		ClassicMiniGames.RemoveBackButtonHandler(OnOKClicked);
		UIManager.Instance.PopScreen();
	}
}

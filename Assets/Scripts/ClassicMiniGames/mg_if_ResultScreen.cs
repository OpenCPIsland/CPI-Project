using ClubPenguin.Classic;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using IceFishing;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_if_ResultScreen : MinigameScreen
{
	public string Result_Token = "Activity.MiniGames.FishResults";

	private Button m_btnExit;

	private Button m_btnReplay;

	public bool Restart
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		UpdatePenguinColor();
		UpdateResultText();
		UpdateButtons();
		AddEvents();
		MinigameManager.GetActive().PlaySFX("mg_if_sfx_UIGameOver");
	}

	public override void UnloadUI()
	{
		RemoveEvents();
		m_btnExit = null;
		m_btnReplay = null;
		base.UnloadUI();
	}

	private void UpdateButtons()
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			switch (button.gameObject.name)
			{
			case "btn_exit":
				m_btnExit = button;
				break;
			case "btn_replay":
				m_btnReplay = button;
				break;
			}
		}
	}

	private void UpdatePenguinColor()
	{
		Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>();
		Image[] array = componentsInChildren;
		int num = 0;
		Image image;
		while (true)
		{
			if (num < array.Length)
			{
				image = array[num];
				if (image.gameObject.name == "penguin_color")
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		image.color = MinigameManager.Instance.GetPenguinColor();
	}

	private void UpdateResultText()
	{
		Text[] componentsInChildren = base.gameObject.GetComponentsInChildren<Text>();
		Text[] array = componentsInChildren;
		int num = 0;
		Text text;
		while (true)
		{
			if (num < array.Length)
			{
				text = array[num];
				if (text.gameObject.name == "txt_result")
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		int fishCaught = MinigameManager.GetActive<mg_IceFishing>().Logic.FishCaught;
		int num2 = 4;
		if (Service.IsSet<Localizer>())
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(Result_Token);
			text.text = string.Format(tokenTranslation, fishCaught, num2, fishCaught * num2);
		}
		else
		{
			text.text = fishCaught + " yellow fish X " + num2 + " = " + fishCaught * num2 + " coins";
		}
	}

	private void AddEvents()
	{
		m_btnExit.onClick.AddListener(OnExitClicked);
		m_btnReplay.onClick.AddListener(OnReplayClicked);
		ClassicMiniGames.PushBackButtonHandler(OnExitClicked);
	}

	private void RemoveEvents()
	{
		m_btnExit.onClick.RemoveListener(OnExitClicked);
		m_btnReplay.onClick.RemoveListener(OnReplayClicked);
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}

	private void OnExitClicked()
	{
		Restart = false;
		UIManager.Instance.PopScreen();
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}

	private void OnReplayClicked()
	{
		Restart = true;
		UIManager.Instance.PopScreen();
	}
}

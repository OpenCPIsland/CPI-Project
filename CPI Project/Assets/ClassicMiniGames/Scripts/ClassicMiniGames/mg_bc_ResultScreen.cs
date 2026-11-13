using BeanCounter;
using ClubPenguin.Classic;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_bc_ResultScreen : MinigameScreen
{
	[Header("Localization Tokens")]
	public string CongratulationsToken = "Activity.MiniGames.Congratulations";

	public string UnloadedTrucksToken = "Activity.MiniGames.Bonus";

	public string TitleToken = "Activity.MiniGames.BeanCounters";

	public string TryAgainToken = "Activity.MiniGames.TryAgain";

	private Text m_titleLabel;

	private Text m_bodyLabel;

	private Button m_replayButton;

	private Button m_exitButton;

	private Localizer localizer;

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
		if (Service.IsSet<Localizer>())
		{
			localizer = Service.Get<Localizer>();
		}
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.HasPauseButton = false;
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_replayButton.onClick.RemoveListener(OnReplayClicked);
		m_exitButton.onClick.RemoveListener(OnExitClicked);
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}

	private void Start()
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_bc_exit_button")
			{
				m_exitButton = button;
			}
			else if (button.gameObject.name == "mg_bc_replay_button")
			{
				m_replayButton = button;
			}
		}
		m_replayButton.onClick.AddListener(OnReplayClicked);
		m_exitButton.onClick.AddListener(OnExitClicked);
		ClassicMiniGames.PushBackButtonHandler(OnExitClicked);
		Text[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Text>();
		Text[] array2 = componentsInChildren2;
		foreach (Text text in array2)
		{
			if (text.gameObject.name == "mg_bc_result_title")
			{
				m_titleLabel = text;
			}
			else if (text.gameObject.name == "mg_bc_result_body")
			{
				m_bodyLabel = text;
			}
		}
		UpdateLabels();
	}

	private void UpdateLabels()
	{
		if (MinigameManager.GetActive<mg_BeanCounter>().GameLogic.DidWin)
		{
			if (localizer != null)
			{
				m_titleLabel.text = localizer.GetTokenTranslation(CongratulationsToken);
				string tokenTranslation = localizer.GetTokenTranslation(UnloadedTrucksToken);
				m_bodyLabel.text = string.Format(tokenTranslation, mg_bc_ScoreController.Instance.GetVictoryBonus());
			}
			else
			{
				m_titleLabel.text = "Congratulations!";
				m_bodyLabel.text = "You have unloaded all of the trucks! You get " + mg_bc_ScoreController.Instance.GetVictoryBonus() + " bonus coins!";
			}
		}
		else if (localizer != null)
		{
			m_titleLabel.text = localizer.GetTokenTranslation(TitleToken);
			m_bodyLabel.text = localizer.GetTokenTranslation(TryAgainToken);
		}
		else
		{
			m_titleLabel.text = "Bean Counter";
			m_bodyLabel.text = "Try Again...";
		}
	}

	private void OnReplayClicked()
	{
		UIManager.Instance.PopScreen();
		MinigameManager.GetActive<mg_BeanCounter>().ReturnToTitle();
	}

	private void OnExitClicked()
	{
		UIManager.Instance.PopScreen();
		MinigameManager.Instance.OnMinigameEnded();
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}
}

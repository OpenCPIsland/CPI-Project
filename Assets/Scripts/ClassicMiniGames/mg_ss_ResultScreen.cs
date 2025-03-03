using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using SmoothieSmash;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_ss_ResultScreen : MinigameScreen
{
	private Button m_btnExit;

	private Button m_btnReplay;

	private float m_timeScale;

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

	protected void Start()
	{
		m_timeScale = Time.timeScale;
		Time.timeScale = 0f;
	}

	protected void OnDestroy()
	{
		Time.timeScale = m_timeScale;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.HasPauseButton = false;
		base.LoadUI(propertyList);
		UpdateResults();
		UpdateButtons();
		AddEvents();
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
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
		Restart = false;
		UIManager.Instance.PopScreen();
	}

	private void OnReplayClicked()
	{
		Restart = true;
		UIManager.Instance.PopScreen();
	}

	private void UpdateResults()
	{
		mg_ss_Score scoring = MinigameManager.GetActive<mg_SmoothieSmash>().GameLogic.Scoring;
		Text[] componentsInChildren = GetComponentsInChildren<Text>();
		Text[] array = componentsInChildren;
		foreach (Text text in array)
		{
			switch (text.gameObject.name)
			{
			case "custom":
				text.text = scoring.CustomText;
				break;
			case "custom_value":
				text.text = scoring.CustomValue.ToString();
				break;
			case "score_value":
				text.text = scoring.Score.ToString();
				break;
			case "coins_value":
				text.text = scoring.Coins.ToString();
				break;
			}
		}
	}
}

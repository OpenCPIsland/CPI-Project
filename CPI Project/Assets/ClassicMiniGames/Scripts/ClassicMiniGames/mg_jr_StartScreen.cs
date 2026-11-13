using DisneyMobile.CoreUnitySystems;
using JetpackReboot;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_jr_StartScreen : MinigameScreen
{
	protected mg_JetpackReboot m_miniGame;

	private mg_jr_GameLogic m_gameLogic = null;

	private GameObject m_SpeechContainer = null;

	private Text m_garySpeechText = null;

	private mg_jr_IntroGary m_gary = null;

	private Button m_goalButton;

	private Button m_tapToPlayButton;

	private bool m_isWaitingToStart = false;

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		m_gameLogic = active.GameLogic;
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_jr_TapToPlayArea")
			{
				m_tapToPlayButton = button;
			}
			else if (button.gameObject.name == "mg_jr_GoalsButton")
			{
				m_goalButton = button;
			}
		}
		m_SpeechContainer = base.transform.Find("mg_jr_pf_StartScreen/mg_jr_SpeechBubble").gameObject;
		Assert.NotNull(m_SpeechContainer, "speech container not found");
		m_garySpeechText = m_SpeechContainer.GetComponentsInChildren<Text>(true)[0];
		Assert.NotNull(m_garySpeechText, "Speech label not found");
		m_gary = m_gameLogic.IntroGary;
		m_tapToPlayButton.onClick.AddListener(OnTapToPlayClicked);
		m_goalButton.onClick.AddListener(OnGoalClicked);
		if (m_gary != null)
		{
			m_gary.OnDialogChanged += OnGaryDialogChange;
		}
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_tapToPlayButton.onClick.RemoveListener(OnTapToPlayClicked);
		m_goalButton.onClick.RemoveListener(OnGoalClicked);
		if (m_gary != null)
		{
			m_gary.OnDialogChanged -= OnGaryDialogChange;
		}
	}

	private void Update()
	{
		if (m_isWaitingToStart)
		{
			bool flag = m_gary != null;
			if (!flag || (flag && m_gary.IsIntroFinished))
			{
				UIManager.Instance.PopScreen();
				m_gameLogic.StartGame();
			}
		}
	}

	public override void OnPoppedToTop(string prevTopName)
	{
		base.OnPoppedToTop(prevTopName);
		base.gameObject.SetActive(true);
	}

	private void OnGoalClicked()
	{
		base.gameObject.SetActive(false);
		MinigameManager.GetActive().PauseGame();
	}

	private void OnTapToPlayClicked()
	{
		m_isWaitingToStart = true;
		if (m_gary != null)
		{
			m_gary.CancelIntro();
		}
	}

	private void OnGaryDialogChange(string _newDialog)
	{
		if (_newDialog == null)
		{
			m_SpeechContainer.SetActive(false);
			return;
		}
		m_SpeechContainer.SetActive(true);
		m_garySpeechText.text = _newDialog;
	}
}

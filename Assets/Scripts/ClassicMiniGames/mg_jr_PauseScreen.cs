using DisneyMobile.CoreUnitySystems;
using JetpackReboot;
using MinigameFramework;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class mg_jr_PauseScreen : MinigameScreen
{
	private mg_jr_GameLogic m_gameLogic = null;

	private Button m_retryButton = null;

	private Button m_quitButton = null;

	private Button m_resumeButton = null;

	private Toggle m_musicButton = null;

	private Toggle m_sfxButton = null;

	private List<mg_jr_UIGoalBar> m_goalBars = new List<mg_jr_UIGoalBar>();

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	private void Start()
	{
		mg_JetpackReboot active = MinigameManager.GetActive<mg_JetpackReboot>();
		m_gameLogic = active.GameLogic;
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_jr_RetryButton")
			{
				m_retryButton = button;
			}
			else if (button.gameObject.name == "mg_jr_QuitButton")
			{
				m_quitButton = button;
			}
			else if (button.gameObject.name == "mg_jr_ResumeButton")
			{
				m_resumeButton = button;
			}
		}
		Toggle[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Toggle>();
		Toggle[] array2 = componentsInChildren2;
		foreach (Toggle toggle in array2)
		{
			if (toggle.gameObject.name == "mg_jr_SfxButton")
			{
				m_sfxButton = toggle;
				m_sfxButton.isOn = m_gameLogic.MiniGame.AreSFXEnabled;
			}
			else if (toggle.gameObject.name == "mg_jr_MusicButton")
			{
				m_musicButton = toggle;
				m_musicButton.isOn = m_gameLogic.MiniGame.MusicManager.IsMusicEnabled;
			}
		}
		m_goalBars.AddRange(from x in base.gameObject.GetComponentsInChildren<mg_jr_UIGoalBar>()
			orderby x.transform.position.y
			select x);
		IList<mg_jr_Goal> activeGoals = active.GoalManager.ActiveGoals;
		Assert.AreEqual(m_goalBars.Count, activeGoals.Count, "Active goal count does not equal available goal bars");
		for (int j = 0; j < activeGoals.Count; j++)
		{
			Assert.NotNull(activeGoals[j], "Null goal in goal list");
			m_goalBars[j].GoalToDisplay = activeGoals[j];
		}
		m_retryButton.onClick.AddListener(OnRetryClicked);
		m_quitButton.onClick.AddListener(OnQuitClicked);
		m_resumeButton.onClick.AddListener(OnResumeClicked);
		m_musicButton.onValueChanged.AddListener(OnMusicClicked);
		m_sfxButton.onValueChanged.AddListener(OnSfxClicked);
		active.MusicManager.IsPaused = true;
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_retryButton.onClick.RemoveListener(OnRetryClicked);
		m_quitButton.onClick.RemoveListener(OnQuitClicked);
		m_resumeButton.onClick.RemoveListener(OnResumeClicked);
		m_musicButton.onValueChanged.RemoveListener(OnMusicClicked);
		m_sfxButton.onValueChanged.RemoveListener(OnSfxClicked);
	}

	private void OnRetryClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		m_gameLogic.MiniGame.MusicManager.IsPaused = false;
		UIManager.Instance.PopScreen();
		m_gameLogic.LoadInitialState();
		m_gameLogic.StartGame();
	}

	private void OnQuitClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		UIManager.Instance.PopScreen();
		MinigameManager.Instance.OnMinigameQuit();
	}

	private void OnResumeClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		m_gameLogic.MiniGame.MusicManager.IsPaused = false;
		UIManager.Instance.PopScreen();
	}

	private void OnMusicClicked(bool p_value)
	{
		MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		m_gameLogic.MiniGame.MusicManager.IsMusicEnabled = p_value;
	}

	private void OnSfxClicked(bool p_value)
	{
		MinigameSFX minigameSFX = MinigameManager.GetActive().PlaySFX("mg_jr_sfx_UISelect");
		m_gameLogic.MiniGame.AreSFXEnabled = p_value;
		if (!m_sfxButton.isOn && minigameSFX != null)
		{
			minigameSFX.AudioTrack.PlayOneShot(minigameSFX.AudioTrack.clip);
		}
	}
}

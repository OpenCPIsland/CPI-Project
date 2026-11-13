using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_PauseScreen : MinigameScreen
{
	private Button m_btnExit;

	private Button m_btnContinue;

	protected override void Awake()
	{
		base.Awake();
		base.HasPauseButton = false;
		base.ShouldShowPauseOver = false;
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "btn_exit")
			{
				m_btnExit = button;
			}
			else if (button.gameObject.name == "btn_continue")
			{
				m_btnContinue = button;
			}
		}
		AddEvents();
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		RemoveEvents();
		base.UnloadUI();
	}

	private void AddEvents()
	{
		m_btnContinue.onClick.AddListener(OnContinueClicked);
		m_btnExit.onClick.AddListener(OnExitClicked);
		ClassicMiniGames.PushBackButtonHandler(OnExitClicked);
	}

	private void RemoveEvents()
	{
		m_btnContinue.onClick.RemoveListener(OnContinueClicked);
		m_btnExit.onClick.RemoveListener(OnExitClicked);
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
	}

	private void OnContinueClicked()
	{
		UIManager.Instance.PopScreen();
	}

	private void OnExitClicked()
	{
		ClassicMiniGames.RemoveBackButtonHandler(OnExitClicked);
		UIManager.Instance.PopScreen();
		MinigameManager.Instance.OnMinigameQuit();
	}
}

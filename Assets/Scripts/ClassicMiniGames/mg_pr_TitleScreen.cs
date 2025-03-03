using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using PuffleRoundup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_pr_TitleScreen : MinigameScreen
{
	private GameObject m_background;

	private Button m_btnPlay;

	private Button m_btnInstructions;

	protected override void Awake()
	{
		base.Awake();
		mg_PuffleRoundup active = MinigameManager.GetActive<mg_PuffleRoundup>();
		m_background = active.Resources.GetInstancedResource(mg_pr_EResourceList.TITLE_ASSET_BACKGROUND);
		MinigameSpriteHelper.FitSpriteToScreen(active.MainCamera, m_background, false);
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_pr_PlayGame")
			{
				m_btnPlay = button;
			}
			else if (button.gameObject.name == "mg_pr_Instructions")
			{
				m_btnInstructions = button;
			}
		}
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnInstructions.onClick.AddListener(OnInstructionsClicked);
	}

	public override void UnloadUI()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnInstructions.onClick.RemoveListener(OnInstructionsClicked);
		base.UnloadUI();
		Object.Destroy(m_background);
		mg_PuffleRoundup active = MinigameManager.GetActive<mg_PuffleRoundup>();
		active.Resources.UnloadResource(mg_pr_EResourceList.TITLE_ASSET_BACKGROUND);
	}

	public void OnPlayClicked()
	{
		UIManager.Instance.PopScreen();
		UIManager.Instance.OpenScreen("mg_pr_GameScreen", false, null, null);
	}

	public void OnInstructionsClicked()
	{
		UIManager.Instance.OpenScreen("mg_pr_InstructionScreen", false, null, null);
		SetButtonsVisible(false);
	}

	public override void CloseScreen()
	{
		base.CloseScreen();
	}

	public override void OnPoppedToTop(string prevTopName)
	{
		SetButtonsVisible(true);
		base.OnPoppedToTop(prevTopName);
	}

	private void SetButtonsVisible(bool p_visible)
	{
		m_btnInstructions.gameObject.SetActive(p_visible);
		m_btnPlay.gameObject.SetActive(p_visible);
	}
}

using BeanCounter;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_bc_InstructionsScreen : MinigameScreen
{
	private Button m_playButton;

	private Button m_backButton;

	private GameObject m_candyUI;

	private GameObject m_normalUI;

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_playButton.onClick.RemoveListener(OnPlayClicked);
		m_backButton.onClick.RemoveListener(OnBackClicked);
	}

	private void Start()
	{
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == "mg_bc_play_button")
			{
				m_playButton = button;
			}
			else if (button.gameObject.name == "mg_bc_back_button")
			{
				m_backButton = button;
			}
		}
		m_playButton.onClick.AddListener(OnPlayClicked);
		m_backButton.onClick.AddListener(OnBackClicked);
		Image[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Image>();
		Image[] array2 = componentsInChildren2;
		foreach (Image image in array2)
		{
			if (image.gameObject.name == "tint")
			{
				image.color = MinigameManager.Instance.GetPenguinColor();
			}
		}
		Transform child = base.gameObject.transform.GetChild(0);
		m_candyUI = child.Find("mg_bc_jelly_instructions").gameObject;
		m_normalUI = child.Find("mg_bc_normal_instructions").gameObject;
		LoadGameMode(MinigameManager.GetActive<mg_BeanCounter>().GameMode);
	}

	private void LoadGameMode(mg_bc_EGameMode _mode)
	{
		m_candyUI.SetActive(_mode != mg_bc_EGameMode.COFFEE_NORMAL);
		m_normalUI.SetActive(_mode == mg_bc_EGameMode.COFFEE_NORMAL);
	}

	private void OnPlayClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UISelect");
		UIManager.Instance.PopScreen();
		UIManager.Instance.PopScreen();
		MinigameManager.GetActive<mg_BeanCounter>().LaunchGame();
	}

	private void OnBackClicked()
	{
		MinigameManager.GetActive().PlaySFX("mg_bc_sfx_UISelect");
		UIManager.Instance.PopScreen();
	}
}

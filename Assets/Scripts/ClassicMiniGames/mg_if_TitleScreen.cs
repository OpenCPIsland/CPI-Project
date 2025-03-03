using IceFishing;
using MinigameFramework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_if_TitleScreen : MinigameScreen
{
	private mg_if_TitleLogic m_titleLogic;

	private GameObject m_title;

	private Transform m_Instructions;

	private Button m_btnPlay;

	private Button m_btnInstructions;

	private Button m_btnBack;

	protected override void Awake()
	{
		base.Awake();
		mg_IceFishing active = MinigameManager.GetActive<mg_IceFishing>();
		m_title = active.Resources.GetInstancedResource(mg_if_EResourceList.TITLE_LOGIC);
		MinigameSpriteHelper.AssignParentTransform(m_title, active.transform);
		m_titleLogic = m_title.GetComponent<mg_if_TitleLogic>();
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		Transform child = base.transform.GetChild(0);
		m_Instructions = child.Find("instructions");
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			switch (button.gameObject.name)
			{
			case "btn_play":
				m_btnPlay = button;
				break;
			case "btn_instructions":
				m_btnInstructions = button;
				break;
			case "btn_back":
				m_btnBack = button;
				break;
			}
		}
		SetInstructionVisibility(false);
		AddEvents();
	}

	public override void UnloadUI()
	{
		RemoveEvents();
		m_btnPlay = null;
		m_btnInstructions = null;
		m_btnBack = null;
		base.UnloadUI();
		Object.Destroy(m_title);
	}

	private void AddEvents()
	{
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnInstructions.onClick.AddListener(OnInstructionsClicked);
		m_btnBack.onClick.AddListener(OnBackClicked);
	}

	private void RemoveEvents()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnInstructions.onClick.RemoveListener(OnInstructionsClicked);
		m_btnBack.onClick.RemoveListener(OnBackClicked);
	}

	private void OnInstructionsClicked()
	{
		SetInstructionVisibility(true);
	}

	private void OnPlayClicked()
	{
		m_titleLogic.PlayGame();
	}

	private void OnBackClicked()
	{
		SetInstructionVisibility(false);
	}

	private void SetInstructionVisibility(bool p_visible)
	{
		m_Instructions.gameObject.SetActive(p_visible);
		m_btnInstructions.gameObject.SetActive(!p_visible);
		m_btnBack.gameObject.SetActive(p_visible);
	}
}

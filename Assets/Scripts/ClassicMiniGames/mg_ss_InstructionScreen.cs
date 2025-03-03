using MinigameFramework;
using SmoothieSmash;
using System.Collections.Generic;
using UnityEngine.UI;

public class mg_ss_InstructionScreen : MinigameScreen
{
	private mg_ss_InstructionLogic m_logic;

	private Button m_btnPlay;

	private Button m_btnBack;

	protected override void Awake()
	{
		m_logic = new mg_ss_InstructionLogic();
		base.Awake();
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			switch (button.gameObject.name)
			{
			case "btn_play":
				m_btnPlay = button;
				break;
			case "btn_back":
				m_btnBack = button;
				break;
			}
		}
		RegisterUIEvents();
	}

	private void RegisterUIEvents()
	{
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnBack.onClick.AddListener(OnBackClicked);
	}

	public override void UnloadUI()
	{
		DeregisterUIEvents();
		m_btnPlay = null;
		m_btnBack = null;
		base.UnloadUI();
	}

	private void DeregisterUIEvents()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnBack.onClick.RemoveListener(OnBackClicked);
	}

	private void OnPlayClicked()
	{
		m_logic.PlayGame();
	}

	private void OnBackClicked()
	{
		m_logic.CloseInstructions();
	}
}

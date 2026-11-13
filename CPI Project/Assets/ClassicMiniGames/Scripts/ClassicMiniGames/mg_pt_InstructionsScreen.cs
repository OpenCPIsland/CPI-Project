using MinigameFramework;
using Pizzatron;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mg_pt_InstructionsScreen : MinigameScreen
{
	private static string NORMAL = "normal";

	private static string CANDY = "cookie";

	private static string BTN_PLAY = "btn_play";

	private static string BTN_BACK = "btn_back";

	private mg_pt_InstructionLogic m_instructionLogic;

	private Transform m_normal;

	private Transform m_candy;

	private Button m_btnPlay;

	private Button m_btnBack;

	protected override void Awake()
	{
		m_instructionLogic = new mg_pt_InstructionLogic();
		base.Awake();
	}

	private void Start()
	{
		Transform child = base.transform.GetChild(0);
		m_normal = child.Find(NORMAL);
		m_candy = child.Find(CANDY);
		UpdateVisibility();
	}

	public override void LoadUI(Dictionary<string, string> propertyList = null)
	{
		base.LoadUI(propertyList);
		Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
		Button[] array = componentsInChildren;
		foreach (Button button in array)
		{
			if (button.gameObject.name == BTN_PLAY)
			{
				m_btnPlay = button;
			}
			else if (button.gameObject.name == BTN_BACK)
			{
				m_btnBack = button;
			}
		}
		RegisterEvents();
	}

	public override void UnloadUI()
	{
		DeregisterEvents();
		m_btnPlay = null;
		m_btnBack = null;
		base.UnloadUI();
	}

	private void RegisterEvents()
	{
		m_btnPlay.onClick.AddListener(OnPlayClicked);
		m_btnBack.onClick.AddListener(OnBackClicked);
	}

	private void DeregisterEvents()
	{
		m_btnPlay.onClick.RemoveListener(OnPlayClicked);
		m_btnBack.onClick.RemoveListener(OnBackClicked);
	}

	private void UpdateVisibility()
	{
		m_candy.gameObject.SetActive(m_instructionLogic.IsCandy);
		m_normal.gameObject.SetActive(!m_instructionLogic.IsCandy);
	}

	private void OnPlayClicked()
	{
		m_instructionLogic.PlayGame();
	}

	private void OnBackClicked()
	{
		m_instructionLogic.CloseInstructions();
	}
}
